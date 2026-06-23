package ru.department.zaderhania_web.controller;

import jakarta.servlet.http.HttpServletResponse;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.xssf.usermodel.XSSFSheet;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import ru.department.zaderhania_web.dto.ReportOfficerWorkloadDto;
import ru.department.zaderhania_web.service.ReportOfficerWorkloadService;

import java.io.IOException;
import java.util.List;

@Controller
@RequestMapping("/reports/officer-workload")
public class ReportOfficerWorkloadController {

    private final ReportOfficerWorkloadService service;

    public ReportOfficerWorkloadController(
            ReportOfficerWorkloadService service
    ) {
        this.service = service;
    }

    @GetMapping
    public String view(Model model) {

        model.addAttribute(
                "rows",
                service.getAll()
        );

        return "reports/officer-workload";
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
                "attachment; filename=report_officer_workload.xlsx"
        );

        List<ReportOfficerWorkloadDto> data =
                service.getAll();

        XSSFWorkbook workbook =
                new XSSFWorkbook();

        XSSFSheet sheet =
                workbook.createSheet("Workload");

        Row header =
                sheet.createRow(0);

        header.createCell(0).setCellValue("ID");
        header.createCell(1).setCellValue("Жетон");
        header.createCell(2).setCellValue("ФИО");
        header.createCell(3).setCellValue("Должность");
        header.createCell(4).setCellValue("Звание");
        header.createCell(5).setCellValue("Активен");
        header.createCell(6).setCellValue("Дел");
        header.createCell(7).setCellValue("Роли");
        header.createCell(8).setCellValue("Задержанных");
        header.createCell(9).setCellValue("Мер");

        int rowNum = 1;

        for (ReportOfficerWorkloadDto r : data) {

            Row row =
                    sheet.createRow(rowNum++);

            row.createCell(0)
                    .setCellValue(r.getOfficerId());

            row.createCell(1)
                    .setCellValue(r.getBadgeNumber());

            row.createCell(2)
                    .setCellValue(r.getFullName());

            row.createCell(3)
                    .setCellValue(r.getPositionName());

            row.createCell(4)
                    .setCellValue(r.getRankName());

            row.createCell(5)
                    .setCellValue(
                            r.isActive()
                                    ? "Да"
                                    : "Нет"
                    );

            row.createCell(6)
                    .setCellValue(r.getCasesCount());

            row.createCell(7)
                    .setCellValue(r.getRoles());

            row.createCell(8)
                    .setCellValue(
                            r.getDetaineesHandled()
                    );

            row.createCell(9)
                    .setCellValue(
                            r.getMeasuresInCases()
                    );
        }

        workbook.write(
                response.getOutputStream()
        );

        workbook.close();
    }
}