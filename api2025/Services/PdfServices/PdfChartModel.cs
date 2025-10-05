namespace api2025.Services.PdfServices;

public class PdfChartModel
{
    public int dodatkowe_lata { get; set; }
    public int rok_emerytury { get; set; }
    public int wiek { get; set; }
    public double emerytura_nominalna_brutto { get; set; }
    public double emerytura_nominalna_netto { get; set; }
    public double emerytura_urealniona_brutto { get; set; }
    public double emerytura_urealniona_netto { get; set; }
    public double wzrost_procent_brutto { get; set; }
    public double wzrost_procent_netto { get; set; }
    public double kapital_emerytalny { get; set; }
    public double srednie_dalsze_trwanie_zycia { get; set; }
}