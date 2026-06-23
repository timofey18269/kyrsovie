package ru.department.zaderhania_web.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import ru.department.zaderhania_web.model.Witness;
import ru.department.zaderhania_web.service.WitnessService;

@Controller
@RequestMapping("/witnesses")
public class WitnessController {

    private final WitnessService service;

    @Autowired
    public WitnessController(WitnessService service) {
        this.service = service;
    }

    @GetMapping
    public String list(Model model) {

        model.addAttribute(
                "witnesses",
                service.getAll()
        );

        return "witnesses/list";
    }

    @GetMapping("/create")
    public String createForm(Model model) {

        model.addAttribute(
                "witness",
                new Witness()
        );

        return "witnesses/form";
    }

    @PostMapping("/create")
    public String create(
            @ModelAttribute Witness witness
    ) {

        service.create(witness);

        return "redirect:/witnesses";
    }

    @GetMapping("/edit/{id}")
    public String editForm(
            @PathVariable int id,
            Model model
    ) {

        model.addAttribute(
                "witness",
                service.getById(id)
        );

        return "witnesses/form";
    }

    @PostMapping("/edit/{id}")
    public String edit(
            @PathVariable int id,
            @ModelAttribute Witness witness
    ) {

        witness.setWitnessId(id);

        service.update(witness);

        return "redirect:/witnesses";
    }

    @PostMapping("/delete/{id}")
    public String delete(
            @PathVariable int id
    ) {

        service.delete(id);

        return "redirect:/witnesses";
    }
}