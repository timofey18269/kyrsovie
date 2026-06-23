package ru.department.zaderhania_web.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import ru.department.zaderhania_web.model.ViolationReport;
import ru.department.zaderhania_web.service.OffenseTypeService;
import ru.department.zaderhania_web.service.ViolationReportService;
import ru.department.zaderhania_web.service.WitnessService;

@Controller
@RequestMapping("/reports")
public class ViolationReportController {

    private final ViolationReportService reportService;
    private final WitnessService witnessService;
    private final OffenseTypeService offenseTypeService;

    @Autowired
    public ViolationReportController(
            ViolationReportService reportService,
            WitnessService witnessService,
            OffenseTypeService offenseTypeService
    ) {
        this.reportService = reportService;
        this.witnessService = witnessService;
        this.offenseTypeService = offenseTypeService;
    }

    @GetMapping
    public String list(Model model) {

        model.addAttribute(
                "reports",
                reportService.getAll()
        );

        return "reports/list";
    }

    @GetMapping("/create")
    public String createForm(Model model) {

        model.addAttribute(
                "report",
                new ViolationReport()
        );

        model.addAttribute(
                "witnesses",
                witnessService.getAll()
        );

        model.addAttribute(
                "offenseTypes",
                offenseTypeService.getAll()
        );

        return "reports/form";
    }

    @GetMapping("/edit/{id}")
    public String editForm(
            @PathVariable int id,
            Model model
    ) {

        model.addAttribute(
                "report",
                reportService.getById(id)
        );

        model.addAttribute(
                "witnesses",
                witnessService.getAll()
        );

        model.addAttribute(
                "offenseTypes",
                offenseTypeService.getAll()
        );

        return "reports/form";
    }

    @PostMapping("/create")
    public String create(
            @ModelAttribute ViolationReport report
    ) {

        reportService.create(report);

        return "redirect:/reports";
    }

    @PostMapping("/edit/{id}")
    public String edit(
            @PathVariable int id,
            @ModelAttribute ViolationReport report
    ) {

        report.setReportId(id);

        reportService.update(report);

        return "redirect:/reports";
    }

    @PostMapping("/delete/{id}")
    public String delete(
            @PathVariable int id
    ) {

        reportService.delete(id);

        return "redirect:/reports";
    }
}