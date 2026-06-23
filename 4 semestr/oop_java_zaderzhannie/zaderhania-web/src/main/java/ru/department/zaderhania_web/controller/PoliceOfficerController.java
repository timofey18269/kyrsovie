package ru.department.zaderhania_web.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import ru.department.zaderhania_web.model.PoliceOfficer;
import ru.department.zaderhania_web.service.PoliceOfficerService;
import ru.department.zaderhania_web.service.PositionService;

@Controller
@RequestMapping("/police")
public class PoliceOfficerController {

    private final PoliceOfficerService officerService;
    private final PositionService positionService;

    @Autowired
    public PoliceOfficerController(
            PoliceOfficerService officerService,
            PositionService positionService
    ) {
        this.officerService = officerService;
        this.positionService = positionService;
    }

    @GetMapping
    public String list(Model model) {

        model.addAttribute(
                "officers",
                officerService.getAll()
        );

        return "police/list";
    }

    @GetMapping("/create")
    public String createForm(Model model) {

        model.addAttribute("officer", new PoliceOfficer());
        model.addAttribute("positions", positionService.getAll());

        return "police/form";
    }

    @PostMapping("/create")
    public String create(
            @ModelAttribute PoliceOfficer officer
    ) {

        officerService.create(officer);

        return "redirect:/police";
    }

    @GetMapping("/edit/{id}")
    public String editForm(
            @PathVariable int id,
            Model model
    ) {

        model.addAttribute(
                "officer",
                officerService.getById(id)
        );

        model.addAttribute(
                "positions",
                positionService.getAll()
        );

        return "police/form";
    }

    @PostMapping("/edit/{id}")
    public String edit(
            @PathVariable int id,
            @ModelAttribute PoliceOfficer officer
    ) {

        officer.setOfficerId(id);

        officerService.update(officer);

        return "redirect:/police";
    }

    @PostMapping("/delete/{id}")
    public String delete(
            @PathVariable int id
    ) {

        officerService.delete(id);

        return "redirect:/police";
    }
}