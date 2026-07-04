
import sys
from pathlib import Path

import numpy as np
import pandas as pd
import statsmodels.api as sm

import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import matplotlib.ticker as mticker

INPUT_PATH = Path(sys.argv[1]) if len(sys.argv) > 1 else Path("offers_combined_1.csv")
OUTPUT_DIR = Path("output_regression")
OUTPUT_DIR.mkdir(exist_ok=True)

ALPHA = 0.05

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

MIN_PLAUSIBLE_PRICE = 1_000_000

plt.rcParams.update({
    "font.family": "DejaVu Sans",
    "axes.titlesize": 13,
    "axes.labelsize": 11,
    "figure.dpi": 150,
})

# Подписи осей для зависимой переменной в зависимости от модели
Y_AXIS_LABELS = {
    "цена": "Цена, млн руб.",
    "ln_цена": "ln(Цена)",
}

# Масштаб для перевода зависимой переменной в удобные для графиков единицы
Y_AXIS_SCALE = {
    "цена": 1_000_000,
    "ln_цена": 1,
}


def thousands_formatter(x, _pos):
    return f"{x:,.0f}".replace(",", " ")


def load_data(path: Path) -> pd.DataFrame:
    df = pd.read_csv(path, sep=None, engine="python", encoding="utf-8-sig")
    df.columns = [c.strip() for c in df.columns]
    df = df[df["цена"] >= MIN_PLAUSIBLE_PRICE].reset_index(drop=True)
    return df


def engineer_features(df: pd.DataFrame) -> pd.DataFrame:
    df = df.copy()
    df["первый_этаж"] = (df["этаж"] == 1).astype(int)
    df["ln_цена"] = np.log(df["цена"])
    df["ln_площадь"] = np.log(df["общая площадь"])
    df["ln_кухня"] = np.log(df["площадь кухни"])
    return df


def fit_model(df: pd.DataFrame, y_col: str, x_cols: list[str]):
    X = sm.add_constant(df[x_cols])
    y = df[y_col]
    model = sm.OLS(y, X).fit()
    return model


def coefficients_table(model, x_cols: list[str]) -> pd.DataFrame:
    names = ["Константа"] + x_cols
    table = pd.DataFrame({
        "Переменная": names,
        "Коэффициент": model.params.values,
        "Стандартная ошибка": model.bse.values,
        "t-статистика": model.tvalues.values,
        "p-value": model.pvalues.values,
    })
    table["Значимость"] = np.where(table["p-value"] < ALPHA, "значим", "не значим")
    table = table.round({
        "Коэффициент": 3, "Стандартная ошибка": 3,
        "t-статистика": 3, "p-value": 4,
    })
    return table


def plot_actual_vs_predicted(y_actual, y_pred, y_col: str, model_index: int):
    scale = Y_AXIS_SCALE.get(y_col, 1)
    y_actual_s = y_actual / scale
    y_pred_s = y_pred / scale

    fig, ax = plt.subplots(figsize=(7, 6))
    ax.scatter(y_actual_s, y_pred_s, alpha=0.7, edgecolor="white")
    lims = [min(y_actual_s.min(), y_pred_s.min()), max(y_actual_s.max(), y_pred_s.max())]
    ax.plot(lims, lims, "r--", linewidth=1.5)
    ax.set_title(f"Прогнозные - фактические значения (Модель {model_index})")
    ax.set_xlabel(f"Фактические значения, {Y_AXIS_LABELS.get(y_col, y_col)}")
    ax.set_ylabel(f"Прогнозные значения, {Y_AXIS_LABELS.get(y_col, y_col)}")
    ax.grid(True, alpha=0.3)
    fig.tight_layout()
    fig.savefig(OUTPUT_DIR / f"actual_vs_predicted_model{model_index}.png")
    plt.close(fig)


def plot_residuals_histogram(residuals, y_col: str, model_index: int):
    scale = Y_AXIS_SCALE.get(y_col, 1)
    residuals_s = residuals / scale

    fig, ax = plt.subplots(figsize=(7, 6))
    ax.hist(residuals_s, bins=20, color="#4C72B0", edgecolor="white")
    ax.set_title(f"Гистограмма распределения ошибок (Модель {model_index})")
    ax.set_xlabel(f"Ошибка (остаток), {Y_AXIS_LABELS.get(y_col, y_col)}")
    ax.set_ylabel("Частота")
    ax.grid(True, alpha=0.3)
    fig.tight_layout()
    fig.savefig(OUTPUT_DIR / f"residuals_hist_model{model_index}.png")
    plt.close(fig)


def print_model_report(name: str, model, x_cols: list[str], table: pd.DataFrame):
    print(f"\n=== {name} ===")
    print(table.to_string(index=False))
    print(f"R² = {model.rsquared:.3f}; Adj. R² = {model.rsquared_adj:.3f}; n = {int(model.nobs)}")


def main():
    df = load_data(INPUT_PATH)
    df = engineer_features(df)

    models_spec = [
        # (имя модели, зависимая переменная, список объясняющих переменных)
        ("Модель 1: Цена ~ Площадь",
         "цена", ["общая площадь"]),

        ("Модель 2: Цена ~ Площадь + Комнаты + Площадь кухни",
         "цена", ["общая площадь", "количество комнат", "площадь кухни"]),

        ("Модель 3: Цена ~ Жилая площадь + Комнаты + Расстояние до транспорта",
         "цена", ["жилая площадь", "количество комнат", "расстояние до общественного транспорта"]),

        ("Модель 4: Цена ~ Площадь + Первый этаж + Этажность дома",
         "цена", ["общая площадь", "первый_этаж", "общая этажность дома"]),

        ("Модель 5: ln(Цена) ~ ln(Площадь) + ln(Площадь кухни) + Комнаты",
         "ln_цена", ["ln_площадь", "ln_кухня", "количество комнат"]),
    ]

    summary_rows = []

    for i, (name, y_col, x_cols) in enumerate(models_spec, start=1):
        model = fit_model(df, y_col, x_cols)
        table = coefficients_table(model, x_cols)
        print_model_report(name, model, x_cols, table)

        out_path = OUTPUT_DIR / f"model{i}_coefficients.csv"
        table.to_csv(out_path, index=False, encoding="utf-8-sig")

        # Диагностические графики: прогноз vs факт и гистограмма остатков
        y_actual = df[y_col]
        y_pred = model.predict(sm.add_constant(df[x_cols]))
        residuals = y_actual - y_pred
        plot_actual_vs_predicted(y_actual, y_pred, y_col, i)
        plot_residuals_histogram(residuals, y_col, i)

        summary_rows.append({
            "Модель": name,
            "Зависимая переменная": y_col,
            "Число факторов": len(x_cols),
            "R²": round(model.rsquared, 3),
            "Adj. R²": round(model.rsquared_adj, 3),
            "n": int(model.nobs),
        })

    summary = pd.DataFrame(summary_rows)
    summary_path = OUTPUT_DIR / "models_summary.csv"
    summary.to_csv(summary_path, index=False, encoding="utf-8-sig")

    print("\n=== Сводная таблица по моделям ===")
    print(summary.to_string(index=False))
    print(f"\nВсе таблицы сохранены в папке: {OUTPUT_DIR.resolve()}")


if __name__ == "__main__":
    main()