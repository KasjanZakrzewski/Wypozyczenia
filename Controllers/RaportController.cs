using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Data;
using WypozyczeniaAPI.Data;
using WypozyczeniaAPI.Models;
using WypozyczeniaAPI.Models.Dto;
using WypozyczeniaAPI.Services;


namespace WypozyczeniaAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RaportController : Controller
    {

        private readonly DBContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public RaportController(DBContext context, IMapper mapper, IEmailService emailService)
        {
            _context = context;
            _mapper = mapper;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: RaportPojazd/Id
        // Metoda pośrednicząca w generacji raportu 
        public ActionResult RaportPojazd(int Id)
        {
            // Generujemy raport PDF za pomocą iTextSharp
            byte[] pdf = GeneratePdfReport(Id).Result;

            // Wysyłamy plik PDF do przeglądarki użytkownika
            return File(pdf, "application/pdf", "Raport.pdf");
        }

        private async Task<byte[]> GeneratePdfReport(int Id)
        {
            var id = Id;
            var i = 1;
            var dystans = 0.0;
            var oplata = 0.0;
            var czas = new TimeSpan();
            var zakonczone = 0;
            var rozpoczete = 0;
            czas = TimeSpan.Zero;

            // Pobieramy dane dotyczące wskazango pojazdu
            var pojazd = await _context.Pojazd.Where(p => p.Id == id).FirstOrDefaultAsync();

            // Pobieramy liste wypożyczeń dotycząctych pojazdu,
            // następnie mapujemy na odpowienie Dto
            var wypozyczenia = await _context.Wypozyczenie.Include(w => w.Uzytkownik).Include(w => w.Pojazd).Where(w => w.PojazdId == id && w.DataZakończenia != null).ToListAsync();
            var wypozyczenieDto = _mapper.Map<List<WypoRaportDto>>(wypozyczenia);

            // Pobieramy liste rezerwacji dotycząctych pojazdu,
            // następnie mapujemy na odpowienie Dto
            var rezerwacje = await _context.Rezerwacja.Include(w => w.Uzytkownik).Include(w => w.Pojazd).Where(w => w.PojazdId == id && w.DataZakończenia != null).ToListAsync();
            var rezerwacjeDto = _mapper.Map<List<WypoRezDto2>>(rezerwacje);

            // Pobieramy liste prac serwisowych dotycząctych pojazdu,
            // następnie mapujemy na odpowienie Dto
            var serwis = await _context.Serwis.Include(w => w.Pracownik).Include(w => w.Admin).Include(w => w.Pojazd).Where(w => w.PojazdId == id).OrderBy(w => w.Id).ToListAsync();
            var serwisDto = _mapper.Map<List<SerwisDto>>(serwis);

            // Tworzymy plik PDF
            var plik = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Text("Raport pojazdu na dzień: " + DateTime.Now.ToShortDateString())
                    .SemiBold().FontSize(24);

                    page.Content().Column(column =>
                    {
                        column.Item().Text(text =>
                        {
                            text.Span("Marka: ").SemiBold();
                            text.Span(pojazd.Marka);
                        });
                        column.Item().Text(text =>
                        {
                            text.Span("Model: ").SemiBold();
                            text.Span(pojazd.Model);
                        });
                        column.Item().Text(text =>
                        {
                            text.Span("Rejestracja: ").SemiBold();
                            text.Span(pojazd.Rejestracja);
                        });

                        column.Item().PaddingTop(5).Text("Wypożyczenia: ")
                        .SemiBold().FontSize(20);
                        column.Item().Table(table =>
                        {
                            
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Row((uint)i).Column(1).BorderTop(1).BorderBottom(1).Text("Data Rozpoczęcia");
                            table.Cell().Row((uint)i).Column(2).BorderTop(1).BorderBottom(1).Text("DataZakończenia");
                            table.Cell().Row((uint)i).Column(3).BorderTop(1).BorderBottom(1).Text("Email");
                            table.Cell().Row((uint)i).Column(4).BorderTop(1).BorderBottom(1).Text("Dystans w km");
                            table.Cell().Row((uint)i).Column(5).BorderTop(1).BorderBottom(1).Text("Oplata w zł");
                            i = 2;
                            foreach (var item in wypozyczenieDto)
                            {
                                table.Cell().Row((uint)i).Column(1).Text(item.DataRozpoczęcia.ToShortDateString());
                                table.Cell().Row((uint)i).Column(2).Text(((DateTime)item.DataZakończenia).ToShortDateString());
                                table.Cell().Row((uint)i).Column(3).Text(item.Email);
                                table.Cell().Row((uint)i).Column(4).Text(Math.Round(item.Dystans, 3));
                                dystans += item.Dystans;
                                table.Cell().Row((uint)i).Column(5).Text(item.Oplata);
                                oplata += item.Oplata;
                                i++;

                                czas += (DateTime)item.DataZakończenia - item.DataRozpoczęcia;

                                /*foreach (var i in Enumerable.Range(1, 5))
                                {
                                    grid.Item(1).Text($"{j}.{i}");
                                }*/
                            }
                            table.Cell().Row((uint)i).Column(1).BorderBottom(1).AlignCenter().Text("-");
                            table.Cell().Row((uint)i).Column(2).BorderBottom(1).AlignCenter().Text("-");
                            table.Cell().Row((uint)i).Column(3).BorderBottom(1).AlignCenter().Text("-");
                            table.Cell().Row((uint)i).Column(4).BorderBottom(1).AlignCenter().Text("-");
                            table.Cell().Row((uint)i).Column(5).BorderBottom(1).AlignCenter().Text("-");
                        });
                        dystans = Math.Round(dystans, 3);
                        oplata = Math.Round(oplata, 2);
                        column.Item().Text("Łączny czas wypożyczenia: " + czas);
                        column.Item().Text("Łącznie przejechał: "+ dystans +" km");
                        column.Item().Text("Łącznie wydano: " + oplata + " zł");


                        column.Item().PaddingTop(5).Text("Rezerwacje: ")
                        .SemiBold().FontSize(20);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Row(1).Column(1).BorderTop(1).BorderBottom(1).Text("Data Rozpoczęcia");
                            table.Cell().Row(1).Column(2).BorderTop(1).BorderBottom(1).Text("DataZakończenia");
                            table.Cell().Row(1).Column(3).BorderTop(1).BorderBottom(1).Text("Email");
                            i = 2;
                            czas = TimeSpan.Zero;
                            foreach (var item in rezerwacjeDto)
                            {
                                table.Cell().Row((uint)i).Column(1).Text(item.DataRozpoczęcia.ToShortDateString());
                                table.Cell().Row((uint)i).Column(2).Text(((DateTime)item.DataZakończenia).ToShortDateString());
                                table.Cell().Row((uint)i).Column(3).Text(item.Email);
                                i++;
                                czas += (DateTime)item.DataZakończenia - item.DataRozpoczęcia;
                            }

                            table.Cell().Row((uint)i).Column(1).BorderBottom(1).AlignCenter().Text("-");
                            table.Cell().Row((uint)i).Column(2).BorderBottom(1).AlignCenter().Text("-");
                            table.Cell().Row((uint)i).Column(3).BorderBottom(1).AlignCenter().Text("-");
                        });
                        column.Item().Text("Łączny czas rezerwacji: " + czas);

                        column.Item().PaddingTop(5).Text("Serwisy: ")
                        .SemiBold().FontSize(20);
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            table.Cell().Row(1).Column(1).BorderTop(1).BorderBottom(1).Text("Data Rozpoczęcia");
                            table.Cell().Row(1).Column(2).BorderTop(1).BorderBottom(1).Text("DataZakończenia");
                            table.Cell().Row(1).Column(3).BorderTop(1).BorderBottom(1).Text("Status");
                            table.Cell().Row(1).Column(4).BorderTop(1).BorderBottom(1).Text("Admin");
                            table.Cell().Row(1).Column(5).BorderTop(1).BorderBottom(1).Text("Pracownik");
                            i = 2;
                            czas = TimeSpan.Zero;
                            foreach (var item in serwisDto)
                            {
                                table.Cell().Row((uint)i).Column(1).Text(item.DataRozpoczecia.ToShortDateString());
                                if (item.DataZakonczenia == null)
                                {
                                    table.Cell().Row((uint)i).Column(2).AlignCenter().Text("-");
                                }
                                else
                                {
                                    table.Cell().Row((uint)i).Column(2).Text(((DateTime)item.DataZakonczenia).ToShortDateString());
                                }
                                table.Cell().Row((uint)i).Column(3).Text(item.Status);
                                table.Cell().Row((uint)i).Column(4).Text(item.Admin);
                                table.Cell().Row((uint)i).Column(5).Text(item.Pracownik);
                                i++;
                                if (item.DataZakonczenia != null)
                                {
                                    czas += (DateTime)item.DataZakonczenia - item.DataRozpoczecia;
                                }
                                if (item.Status == "Zakończony")
                                {
                                    zakonczone += 1;
                                }
                                else
                                {
                                    rozpoczete += 1;
                                }
                            }
                            table.Cell().Row((uint)i).Column(1).BorderBottom(1).AlignCenter().Text("-");
                            table.Cell().Row((uint)i).Column(2).BorderBottom(1).AlignCenter().Text("-");
                            table.Cell().Row((uint)i).Column(3).BorderBottom(1).AlignCenter().Text("-");
                            table.Cell().Row((uint)i).Column(4).BorderBottom(1).AlignCenter().Text("-");
                            table.Cell().Row((uint)i).Column(5).BorderBottom(1).AlignCenter().Text("-");
                        });
                        column.Item().Text("Łączny czas zakończonych prac serwisowych: " + czas);
                        column.Item().Text("Ilość zakończonych prac serwisowych: " + zakonczone);
                        column.Item().Text("Ilość aktywnych prac serwisowych: " + rozpoczete);

                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                    });

                });

            }).GeneratePdf();

            // zwracamy gotowy plik 
            return plik;
        }
    }
}
