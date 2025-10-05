using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace api2025.Services.PdfServices.Components;

public class DataBoxComponent : IComponent
{
    private readonly PdfChartModel _s;
    private readonly CultureInfo _pl;

    public DataBoxComponent(PdfChartModel s, CultureInfo pl)
    {
        _s = s;
        _pl = pl;
    }

    public void Compose(IContainer container)
    {
        container
            .Padding(8)
            .Column(col =>
            {
                col.Spacing(2);

                // nagłówek z „ramką” tylko u góry
                col.Item().Padding(6).Background(Colors.Grey.Lighten4)
                    .Text($"Scenariusz: +{_s.dodatkowe_lata} lata | Rok emerytury: {_s.rok_emerytury} | Wiek: {_s.wiek}")
                    .Bold();

                // helper
                void Row(string l, string v) =>
                    col.Item().PaddingVertical(2)
                        .BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)  // ✅ delikatna linia POD wierszem
                        .Text(t => { t.Span(l + ": ").Bold(); t.Span(v); });

                Row("Emerytura nominalna BRUTTO", _s.emerytura_nominalna_brutto.ToString("N2", _pl) + " zł");
                Row("Emerytura nominalna NETTO",  _s.emerytura_nominalna_netto.ToString("N2", _pl) + " zł");
                Row("Emerytura urealniona BRUTTO", _s.emerytura_urealniona_brutto.ToString("N2", _pl) + " zł");
                Row("Emerytura urealniona NETTO",  _s.emerytura_urealniona_netto.ToString("N2", _pl) + " zł");
                Row("Wzrost % BRUTTO",             _s.wzrost_procent_brutto.ToString("N2", _pl) + " %");
                Row("Wzrost % NETTO",              _s.wzrost_procent_netto.ToString("N2", _pl) + " %");
                Row("Kapitał emerytalny",          _s.kapital_emerytalny.ToString("N2", _pl) + " zł");
                Row("Średnie dalsze trwanie życia (mies.)", _s.srednie_dalsze_trwanie_zycia.ToString("N1", _pl));
                col.Item().PaddingVertical(6).LineHorizontal(0.5f);
            });
    }
}
