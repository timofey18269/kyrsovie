package ru.department.zaderhania_web.controller;

import ru.department.zaderhania_web.model.OffenseType;
import ru.department.zaderhania_web.service.OffenseTypeService;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

@Controller
@RequestMapping("/offenses")
public class OffenseTypeController {

    private final OffenseTypeService service;

    @Autowired
    public OffenseTypeController(OffenseTypeService service) {
        this.service = service;
    }

    @GetMapping
    public String list(Model model) {

        model.addAttribute("offenses", service.getAll());

        return "offenses/list";
    }

    @GetMapping("/create")
    public String createForm(Model model) {

        model.addAttribute("offense", new OffenseType());

        return "offenses/form";
    }

    @PostMapping("/create")
    public String create(@ModelAttribute OffenseType offense) {

        service.create(offense);

        return "redirect:/offenses";
    }

    @GetMapping("/edit/{id}")
    public String editForm(@PathVariable int id, Model model) {

        model.addAttribute("offense", service.getById(id));

        return "offenses/form";
    }

    @PostMapping("/edit/{id}")
    public String edit(@PathVariable int id, @ModelAttribute OffenseType offense) {

        offense.setOffenseTypeId(id);

        service.update(offense);

        return "redirect:/offenses";
    }

    @PostMapping("/delete/{id}")
    public String delete(@PathVariable int id) {

        service.delete(id);

        return "redirect:/offenses";
    }
}