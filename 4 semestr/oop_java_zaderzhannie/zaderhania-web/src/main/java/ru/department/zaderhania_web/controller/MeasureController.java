package ru.department.zaderhania_web.controller;

import ru.department.zaderhania_web.model.Measure;
import ru.department.zaderhania_web.service.CaseService;
import ru.department.zaderhania_web.service.DetaineeService;
import ru.department.zaderhania_web.service.MeasureService;

import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

@Controller
@RequestMapping("/measures")
public class MeasureController {

    private final MeasureService measureService;
    private final DetaineeService detaineeService;
    private final CaseService caseService;

    public MeasureController(
            MeasureService measureService,
            DetaineeService detaineeService,
            CaseService caseService
    ) {
        this.measureService = measureService;
        this.detaineeService = detaineeService;
        this.caseService = caseService;
    }

    @GetMapping
    public String list(Model model) {

        model.addAttribute(
                "measures",
                measureService.getAllDetailed()
        );

        return "measures/list";
    }

    @GetMapping("/create")
    public String createForm(Model model) {

        model.addAttribute("measure", new Measure());
        model.addAttribute("detainees", detaineeService.getAll());
        model.addAttribute("cases", caseService.getAll());

        return "measures/form";
    }

    @PostMapping("/create")
    public String create(
            @ModelAttribute Measure measure
    ) {

        measureService.create(measure);

        return "redirect:/measures";
    }

    @GetMapping("/edit/{id}")
    public String editForm(
            @PathVariable int id,
            Model model
    ) {

        model.addAttribute(
                "measure",
                measureService.getById(id)
        );

        model.addAttribute("detainees", detaineeService.getAll());
        model.addAttribute("cases", caseService.getAll());

        return "measures/form";
    }

    @PostMapping("/edit/{id}")
    public String edit(
            @PathVariable int id,
            @ModelAttribute Measure measure
    ) {

        measure.setMeasureId(id);

        measureService.update(measure);

        return "redirect:/measures";
    }

    @PostMapping("/delete/{id}")
    public String delete(@PathVariable int id) {

        measureService.delete(id);

        return "redirect:/measures";
    }
}