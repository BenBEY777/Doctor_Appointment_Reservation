using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using DocTest.Data;
using DocTest.Models;
using OfficeOpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;


public class AppointmentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AppointmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Appointments/Index
    public async Task<IActionResult> Index()
    {
        var appointments = await _context.Appointments.ToListAsync();
        return View(appointments);
    }

    // GET: Appointments/Admin
    public async Task<IActionResult> Admin()
    {
        var appointments = await _context.Appointments.ToListAsync();
        return View(appointments);
    }

    // GET: Appointments/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(m => m.Id == id);

        if (appointment == null)
        {
            return NotFound();
        }

        return View(appointment);
    }

    // GET: Appointments/Create
    public IActionResult Create()
    {
        ViewBag.Doctors = new SelectList(_context.Doctors, "Name", "Name");
        return View();
    }

    // POST: Appointments/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Appointment appointment)
    {
        if (ModelState.IsValid)
        {
            _context.Add(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Doctors = new SelectList(_context.Doctors, "Name", "Name");
        return View(appointment);
    }

    // GET: Appointments/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return NotFound();
        }

        ViewBag.Doctors = new SelectList(_context.Doctors, "Name", "Name", appointment.DoctorName);
        return View(appointment);

    }

    // POST: Appointments/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Appointment appointment)
    {
        if (id != appointment.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(appointment.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Admin));
        }

        ViewBag.Doctors = new SelectList(_context.Doctors, "Name", "Name", appointment.DoctorName);
        return View(appointment);
    }

    // GET: Appointments/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var appointment = await _context.Appointments.FirstOrDefaultAsync(m => m.Id == id);
        if (appointment == null)
        {
            return NotFound();
        }

        return View(appointment);
    }

    // POST: Appointments/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment != null)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Admin));
    }

    private bool AppointmentExists(int id)
    {
        return _context.Appointments.Any(e => e.Id == id);
    }


    public async Task<IActionResult> ExportToWord()
    {
        var appointments = await _context.Appointments.ToListAsync();

        using (var stream = new MemoryStream())
        {
            using (var wordDocument = WordprocessingDocument.Create(stream, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
            {
                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = mainPart.Document.AppendChild(new Body());

                // Заглавие
                body.AppendChild(new Paragraph(new Run(new Text("Списък на Запазени Часове")))
                {
                    ParagraphProperties = new ParagraphProperties(new Justification() { Val = JustificationValues.Center })
                });

                // Таблица
                var table = new Table();

                table.AppendChild(new TableProperties(
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single },
                        new BottomBorder { Val = BorderValues.Single },
                        new LeftBorder { Val = BorderValues.Single },
                        new RightBorder { Val = BorderValues.Single },
                        new InsideHorizontalBorder { Val = BorderValues.Single },
                        new InsideVerticalBorder { Val = BorderValues.Single }
                    )));

                // Заглавен ред
                var headerRow = new TableRow();
                headerRow.Append(
                    new TableCell(new Paragraph(new Run(new Text("Доктор")))),
                    new TableCell(new Paragraph(new Run(new Text("Пациент")))),
                    new TableCell(new Paragraph(new Run(new Text("Дата")))),
                    new TableCell(new Paragraph(new Run(new Text("Час"))))
                );
                table.Append(headerRow);

                // Данни
                foreach (var appointment in appointments)
                {
                    var row = new TableRow();
                    row.Append(
                        new TableCell(new Paragraph(new Run(new Text(appointment.DoctorName)))),
                        new TableCell(new Paragraph(new Run(new Text(appointment.PatientName)))),
                        new TableCell(new Paragraph(new Run(new Text(appointment.AppointmentDate.ToString("dd/MM/yyyy"))))),
                        new TableCell(new Paragraph(new Run(new Text(appointment.AppointmentTime.ToString(@"hh\:mm")))))
                    );
                    table.Append(row);
                }

                body.AppendChild(table);
            }

            stream.Position = 0;
            return File(new MemoryStream(stream.ToArray()),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "Appointments.docx");
        }
    }
}
