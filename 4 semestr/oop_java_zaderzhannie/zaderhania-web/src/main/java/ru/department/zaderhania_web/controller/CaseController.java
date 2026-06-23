package ru.department.zaderhania_web.controller;

import ru.department.zaderhania_web.model.Case;
import ru.department.zaderhania_web.service.CaseService;
import ru.department.zaderhania_web.service.OffenseTypeService;
import ru.department.zaderhania_web.service.ViolationReportService;

import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

@Controller
@RequestMapping("/cases")
public class CaseController {

    private final CaseService caseService;
    private final ViolationReportService reportService;
    private final OffenseTypeService offenseTypeService;

    public CaseController(
            CaseService caseService,
            ViolationReportService reportService,
            OffenseTypeService offenseTypeService
    ) {
        this.caseService = caseService;
        this.reportService = reportService;
        this.offenseTypeService = offenseTypeService;
    }

    @GetMapping
    public String list(Model model) {

        model.addAttribute(
                "cases",
                caseService.getAllDetailed()
        );

        return "cases/list";
    }

    @GetMapping("/details/{id}")
    public String details(
            @PathVariable int id,
            Model model
    ) {

        model.addAttribute(
                "details",
                caseService.getDetails(id)
        );

        return "cases/details";
    }

    @GetMapping("/create")
    public String createForm(Model model) {

        model.addAttribute("caseObj", new Case());

        model.addAttribute(
                "reports",
                reportService.getAll()
        );

        model.addAttribute(
                "offenseTypes",
                offenseTypeService.getAll()
        );

        return "cases/form";
    }

    @PostMapping("/create")
    public String create(
            @ModelAttribute("caseObj") Case caseObj
    ) {

        caseService.create(caseObj);

        return "redirect:/cases";
    }

    @GetMapping("/edit/{id}")
    public String editForm(
            @PathVariable int id,
            Model model
    ) {

        model.addAttribute(
                "caseObj",
                caseService.getById(id)
        );

        model.addAttribute(
                "reports",
                reportService.getAll()
        );

        model.addAttribute(
                "offenseTypes",
                offenseTypeService.getAll()
        );

        return "cases/form";
    }

    @PostMapping("/edit/{id}")
    public String edit(
            @PathVariable int id,
            @ModelAttribute("caseObj") Case caseObj
    ) {

        caseObj.setCaseId(id);

        caseService.update(caseObj);

        return "redirect:/cases";
    }

    @PostMapping("/delete/{id}")
    public String delete(
            @PathVariable int id
    ) {

        caseService.delete(id);

        return "redirect:/cases";
    }

}