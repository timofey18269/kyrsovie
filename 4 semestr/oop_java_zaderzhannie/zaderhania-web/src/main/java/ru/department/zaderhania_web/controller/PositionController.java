package ru.department.zaderhania_web.controller;

import ru.department.zaderhania_web.model.Position;
import ru.department.zaderhania_web.service.PositionService;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

@Controller
@RequestMapping("/positions")
public class PositionController {

    private final PositionService service;

    @Autowired
    public PositionController(PositionService service) {
        this.service = service;
    }

    @GetMapping
    public String list(Model model) {

        model.addAttribute(
                "positions",
                service.getAll()
        );

        return "positions/list";
    }

    @GetMapping("/create")
    public String createForm(Model model) {

        model.addAttribute(
                "position",
                new Position()
        );

        return "positions/form";
    }

    @PostMapping("/create")
    public String create(
            @ModelAttribute Position position
    ) {

        service.create(position);

        return "redirect:/positions";
    }

    @GetMapping("/edit/{id}")
    public String editForm(
            @PathVariable int id,
            Model model
    ) {

        model.addAttribute(
                "position",
                service.getById(id)
        );

        return "positions/form";
    }

    @PostMapping("/edit/{id}")
    public String edit(
            @PathVariable int id,
            @ModelAttribute Position position
    ) {

        position.setPositionId(id);

        service.update(position);

        return "redirect:/positions";
    }

    @PostMapping("/delete/{id}")
    public String delete(
            @PathVariable int id
    ) {

        service.delete(id);

        return "redirect:/positions";
    }
}