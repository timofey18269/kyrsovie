
import sys
from pathlib import Path

import pandas as pd
import matplotlib
matplotlib.use("Agg")  # без графического интерфейса
import matplotlib.pyplot as plt
import matplotlib.ticker as mticker

# ---------------------------------------------------------------------------
# Настройки
# ---------------------------------------------------------------------------

INPUT_PATH = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("offers_combined_1.csv")
OUTPUT_DIR = Path("output_describe_stats")
OUTPUT_DIR.mkdir(exist_ok=True)

KEY_COLUMNS = ["цена", "общая площадь"]

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

plt.rcParams.update({
    "font.family": "DejaVu Sans",
    "axes.titlesize": 14,
    "axes.labelsize": 12,
    "figure.dpi": 150,
})


def load_data(path: Path) -> pd.DataFrame:
    """Читает CSV-файл, автоматически определяя разделитель (',' или ';')."""
    df = pd.read_csv(path, sep=None, engine="python", encoding="utf-8-sig")
    df.columns = [c.strip() for c in df.columns]
    return df


def mode_value(series: pd.Series):
    """Возвращает первую моду (или NaN, если мод несколько и они не выделяются)."""
    m = series.mode()
    return m.iloc[0] if not m.empty else float("nan")


def build_descriptive_table(df: pd.DataFrame, columns: list[str]) -> pd.DataFrame:
    """Строит таблицу описательной статистики по заданным числовым колонкам."""
    rows = {
        "Количество наблюдений": [],
        "Среднее": [],
        "Стандартное отклонение": [],
        "Минимум": [],
        "Первый квартиль": [],
        "Медиана": [],
        "Третий квартиль": [],
        "Максимум": [],
        "Мода": [],
    }
    for col in columns:
        s = df[col].dropna()
        rows["Количество наблюдений"].append(int(s.count()))
        rows["Среднее"].append(round(s.mean(), 2))
        rows["Стандартное отклонение"].append(round(s.std(), 2))
        rows["Минимум"].append(round(s.min(), 2))
        rows["Первый квартиль"].append(round(s.quantile(0.25), 2))
        rows["Медиана"].append(round(s.median(), 2))
        rows["Третий квартиль"].append(round(s.quantile(0.75), 2))
        rows["Максимум"].append(round(s.max(), 2))
        rows["Мода"].append(round(mode_value(s), 2))

    table = pd.DataFrame(rows, index=columns).T
    return table


def plot_histogram(df: pd.DataFrame, column: str, title: str, xlabel: str, filename: str):
    fig, ax = plt.subplots(figsize=(8, 6))
    df[column].dropna().hist(bins=20, ax=ax, color="#4C72B0", edgecolor="white")
    ax.set_title(title)
    ax.set_xlabel(xlabel)
    ax.set_ylabel("Частота")
    ax.xaxis.set_major_formatter(mticker.FuncFormatter(lambda x, _: f"{x:,.0f}".replace(",", " ")))
    fig.tight_layout()
    fig.savefig(OUTPUT_DIR / filename)
    plt.close(fig)


def plot_boxplot(df: pd.DataFrame, column: str, title: str, ylabel: str, filename: str):
    fig, ax = plt.subplots(figsize=(6, 6))
    df.boxplot(column=column, ax=ax)
    ax.set_title(title)
    ax.set_ylabel(ylabel)
    ax.set_xticklabels([""])
    ax.yaxis.set_major_formatter(mticker.FuncFormatter(lambda x, _: f"{x:,.0f}".replace(",", " ")))
    fig.tight_layout()
    fig.savefig(OUTPUT_DIR / filename)
    plt.close(fig)


def main():
    df = load_data(INPUT_PATH)

    missing = [c for c in NUMERIC_COLUMNS if c not in df.columns]
    if missing:
        raise SystemExit(f"В файле отсутствуют ожидаемые колонки: {missing}")

    table = build_descriptive_table(df, NUMERIC_COLUMNS)
    table_path = OUTPUT_DIR / "descriptive_statistics.csv"
    table.to_csv(table_path, encoding="utf-8-sig")
    print(f"Таблица описательной статистики сохранена: {table_path}")
    print(table.to_string())

    plot_histogram(
        df, "цена",
        title="Распределение цен на квартиры на вторичном рынке г. Махачкалы",
        xlabel="Цена, руб.",
        filename="hist_price.png",
    )
    plot_boxplot(
        df, "цена",
        title="Диаграмма размаха цен на квартиры",
        ylabel="Цена, руб.",
        filename="box_price.png",
    )

    plot_histogram(
        df, "общая площадь",
        title="Распределение общей площади квартир на вторичном рынке г. Махачкалы",
        xlabel="Общая площадь, м²",
        filename="hist_area.png",
    )
    plot_boxplot(
        df, "общая площадь",
        title="Диаграмма размаха общей площади квартир",
        ylabel="Общая площадь, м²",
        filename="box_area.png",
    )

    print(f"Графики сохранены в папке: {OUTPUT_DIR.resolve()}")


if __name__ == "__main__":
    main()