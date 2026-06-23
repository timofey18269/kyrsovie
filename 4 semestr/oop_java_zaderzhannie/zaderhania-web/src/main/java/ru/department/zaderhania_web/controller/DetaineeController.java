package ru.department.zaderhania_web.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import ru.department.zaderhania_web.model.Detainee;
import ru.department.zaderhania_web.service.DetaineeService;

@Controller
@RequestMapping("/detainees")
public class DetaineeController {

    private final DetaineeService service;

    @Autowired
    public DetaineeController(DetaineeService service) {
        this.service = service;
    }

    @GetMapping
    public String list(Model model) {

        model.addAttribute(
                "detainees",
                service.getAll()
        );

        return "detainees/list";
    }

    @GetMapping("/create")
    public String createForm(Model model) {

        model.addAttribute(
                "detainee",
                new Detainee()
        );

        return "detainees/form";
    }

    @PostMapping("/create")
    public String create(
            @ModelAttribute Detainee detainee
    ) {

        service.create(detainee);

        return "redirect:/detainees";
    }

    @GetMapping("/edit/{id}")
    public String editForm(
            @PathVariable int id,
            Model model
    ) {

        model.addAttribute(
                "detainee",
                service.getById(id)
        );

        return "detainees/form";
    }

    @PostMapping("/edit/{id}")
    public String edit(
            @PathVariable int id,
            @ModelAttribute Detainee detainee
    ) {

        detainee.setDetaineeId(id);

        service.update(detainee);

        return "redirect:/detainees";
    }

    @PostMapping("/delete/{id}")
    public String delete(
            @PathVariable int id
    ) {

        service.delete(id);

        return "redirect:/detainees";
    }
}