package ru.department.zaderhania_web.controller;

import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.xssf.usermodel.XSSFSheet;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import ru.department.zaderhania_web.dto.ReportCaseSummaryDto;
import ru.department.zaderhania_web.service.ReportCaseSummaryService;

import jakarta.servlet.http.HttpServletResponse;
import java.io.IOException;
import java.util.List;

@Controller
@RequestMapping("/reports/case-summary")
public class ReportCaseSummaryController {

    private final ReportCaseSummaryService service;

    public ReportCaseSummaryController(
            ReportCaseSummaryService service
    ) {
        this.service = service;
    }

    @GetMapping
    public String view(Model model) {

        model.addAttribute(
                "rows",
                service.getAll()
        );

        return "reports/case-summary";
    }

    @GetMapping("/excel")
    public void exportExcel(
            HttpServletResponse response
    ) throws IOException {

        response.setContentType(
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        );

        response.setHeader(
                "Content-Disposition",
                "attachment; filename=report_case_summary.xlsx"
        );

        List<ReportCaseSummaryDto> data = service.getAll();

        XSSFWorkbook workbook = new XSSFWorkbook();
        XSSFSheet sheet = workbook.createSheet("Cases");

        Row header = sheet.createRow(0);

        header.createCell(0).setCellValue("ID");
        header.createCell(1).setCellValue("Номер дела");
        header.createCell(2).setCellValue("Тип");
        header.createCell(3).setCellValue("Статус");
        header.createCell(4).setCellValue("Заявление");
        header.createCell(5).setCellValue("Открыто");
        header.createCell(6).setCellValue("Закрыто");
        header.createCell(7).setCellValue("Полицейских");
        header.createCell(8).setCellValue("Задержанных");
        header.createCell(9).setCellValue("Свидетелей");
        header.createCell(10).setCellValue("Мер");
        header.createCell(11).setCellValue("Типы мер");
        header.createCell(12).setCellValue("Описание");

        int rowNum = 1;

        for (ReportCaseSummaryDto r : data) {

            Row row = sheet.createRow(rowNum++);

            row.createCell(0).setCellValue(r.getCaseId());
            row.createCell(1).setCellValue(r.getCaseNumber());
            row.createCell(2).setCellValue(r.getOffenseType());
            row.createCell(3).setCellValue(r.getCaseStatus());
            row.createCell(4).setCellValue(r.getReportNumber());
            row.createCell(5).setCellValue(String.valueOf(r.getOpenedAt()));
            row.createCell(6).setCellValue(String.valueOf(r.getClosedAt()));
            row.createCell(7).setCellValue(r.getOfficersCount());
            row.createCell(8).setCellValue(r.getDetaineesCount());
            row.createCell(9).setCellValue(r.getWitnessesCount());
            row.createCell(10).setCellValue(r.getMeasuresCount());
            row.createCell(11).setCellValue(r.getMeasureTypes());
            row.createCell(12).setCellValue(r.getDescription());
        }

        workbook.write(response.getOutputStream());
        workbook.close();
    }
}