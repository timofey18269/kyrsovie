
import sys
from pathlib import Path

import numpy as np
import pandas as pd
from scipy.stats import pearsonr, t as t_dist

import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import matplotlib.ticker as mticker
import seaborn as sns

# ---------------------------------------------------------------------------
# Настройки
# ---------------------------------------------------------------------------

INPUT_PATH = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("offers_combined_1.csv")
OUTPUT_DIR = Path("output_correlation")
OUTPUT_DIR.mkdir(exist_ok=True)

ALPHA = 0.05  # уровень значимости

# Все числовые характеристики выборки
NUMERIC_COLUMNS = [
    "цена",
    "общая площадь",
    "жилая площадь",
    "площадь кухни",
    "количество комнат",
    "расстояние до общественного транспорта",
    "этаж",
    "общая этажность дома",
    "высота потолков",
]

TARGET = "цена"

SCATTER_PAIRS = [
    ("общая площадь", "цена"),
    ("количество комнат", "цена"),
    ("расстояние до общественного транспорта", "цена"),
    ("общая этажность дома", "цена"),
    ("общая площадь", "жилая площадь"),
    ("этаж", "общая этажность дома"),
]

SHORT_LABELS = {
    "цена": "Цена, руб.",
    "общая площадь": "Общая площадь, м²",
    "жилая площадь": "Жилая площадь, м²",
    "площадь кухни": "Площадь кухни, м²",
    "количество комнат": "Количество комнат",
    "расстояние до общественного транспорта": "Расстояние до транспорта, м",
    "этаж": "Этаж",
    "общая этажность дома": "Этажность дома",
    "высота потолков": "Высота потолков, м",
}

plt.rcParams.update({
    "font.family": "DejaVu Sans",
    "axes.titlesize": 13,
    "axes.labelsize": 11,
    "figure.dpi": 150,
})


def load_data(path: Path) -> pd.DataFrame:
    """Читает CSV-файл, автоматически определяя разделитель (',' или ';')."""
    df = pd.read_csv(path, sep=None, engine="python", encoding="utf-8-sig")
    df.columns = [c.strip() for c in df.columns]
    return df


def thousands_formatter(x, _pos):
    return f"{x:,.0f}".replace(",", " ")


def build_correlation_matrix(df: pd.DataFrame, columns: list[str]) -> pd.DataFrame:
    return df[columns].corr(method="pearson").round(3)


def plot_heatmap(corr: pd.DataFrame, filename: str):
    fig, ax = plt.subplots(figsize=(10, 8))
    sns.heatmap(
        corr, annot=True, fmt=".2f", cmap="YlOrRd",
        square=True, linewidths=0.5, cbar_kws={"shrink": 0.8}, ax=ax,
    )
    ax.set_title("Матрица корреляций Пирсона")
    fig.tight_layout()
    fig.savefig(OUTPUT_DIR / filename)
    plt.close(fig)


def plot_scatterplots(df: pd.DataFrame, pairs: list[tuple[str, str]], filename: str):
    n = len(pairs)
    ncols = 2
    nrows = int(np.ceil(n / ncols))
    fig, axes = plt.subplots(nrows, ncols, figsize=(12, 4.5 * nrows))
    axes = axes.flatten()

    for ax, (x_col, y_col) in zip(axes, pairs):
        sns.scatterplot(x=df[x_col], y=df[y_col], ax=ax, alpha=0.7, edgecolor="white")
        r, _ = pearsonr(df[x_col], df[y_col])
        ax.set_title(f"{SHORT_LABELS.get(y_col, y_col)} : {SHORT_LABELS.get(x_col, x_col)}\n(r = {r:.2f})")
        ax.set_xlabel(SHORT_LABELS.get(x_col, x_col))
        ax.set_ylabel(SHORT_LABELS.get(y_col, y_col))
        if x_col == "цена" or y_col == "цена":
            ax.yaxis.set_major_formatter(mticker.FuncFormatter(thousands_formatter)) \
                if y_col == "цена" else None
            ax.xaxis.set_major_formatter(mticker.FuncFormatter(thousands_formatter)) \
                if x_col == "цена" else None

    # скрыть лишние оси, если пар меньше, чем ячеек сетки
    for ax in axes[n:]:
        ax.axis("off")

    fig.tight_layout()
    fig.savefig(OUTPUT_DIR / filename)
    plt.close(fig)


def build_significance_table(df: pd.DataFrame, target: str, columns: list[str]) -> pd.DataFrame:
    n = len(df)
    df_freedom = n - 2
    t_crit = t_dist.ppf(1 - ALPHA / 2, df_freedom)

    rows = []
    for col in columns:
        if col == target:
            continue
        r, p_value = pearsonr(df[col], df[target])
        t_stat = r * np.sqrt(df_freedom / (1 - r ** 2))
        significant = abs(t_stat) > t_crit
        rows.append({
            "Переменная": col,
            "r": round(r, 3),
            "t-расчётное": round(t_stat, 2),
            "t-критическое": round(t_crit, 3),
            "p-value": round(p_value, 4) if p_value >= 0.0001 else "< 0,0001",
            "Значимость": "значима" if significant else "не значима",
        })

    table = pd.DataFrame(rows).sort_values("r", key=lambda s: s.abs(), ascending=False)
    return table.reset_index(drop=True)


def main():
    df = load_data(INPUT_PATH)

    missing = [c for c in NUMERIC_COLUMNS if c not in df.columns]
    if missing:
        raise SystemExit(f"В файле отсутствуют ожидаемые колонки: {missing}")

    # 1. Матрица корреляций Пирсона
    corr = build_correlation_matrix(df, NUMERIC_COLUMNS)
    corr_path = OUTPUT_DIR / "correlation_matrix.csv"
    corr.to_csv(corr_path, encoding="utf-8-sig")
    print(f"Матрица корреляций сохранена: {corr_path}")
    print(corr.to_string())

    # 2. Тепловая карта
    plot_heatmap(corr, "heatmap_correlation.png")

    # 3. Диаграммы рассеяния (6 пар переменных)
    plot_scatterplots(df, SCATTER_PAIRS, "scatterplots.png")

    # 4. Проверка статистической значимости корреляции цены со всеми
    #    остальными характеристиками
    sig_table = build_significance_table(df, TARGET, NUMERIC_COLUMNS)
    sig_path = OUTPUT_DIR / "significance_table.csv"
    sig_table.to_csv(sig_path, index=False, encoding="utf-8-sig")
    print(f"\nТаблица значимости корреляций с ценой сохранена: {sig_path}")
    print(sig_table.to_string(index=False))

    print(f"\nГрафики сохранены в папке: {OUTPUT_DIR.resolve()}")


if __name__ == "__main__":
    main()