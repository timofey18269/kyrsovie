package ru.department.zaderhania_web.controller;

import ru.department.zaderhania_web.repository.*;

import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

import java.sql.Connection;
import java.sql.PreparedStatement;

import javax.sql.DataSource;

@Controller
@RequestMapping("/cases/assign")
public class CaseAssignmentController {

    private final DataSource dataSource;

    private final CaseRepository caseRepository;
    private final PoliceOfficerRepository policeRepository;
    private final DetaineeRepository detaineeRepository;
    private final WitnessRepository witnessRepository;

    public CaseAssignmentController(
            DataSource dataSource,
            CaseRepository caseRepository,
            PoliceOfficerRepository policeRepository,
            DetaineeRepository detaineeRepository,
            WitnessRepository witnessRepository
    ) {
        this.dataSource = dataSource;
        this.caseRepository = caseRepository;
        this.policeRepository = policeRepository;
        this.detaineeRepository = detaineeRepository;
        this.witnessRepository = witnessRepository;
    }

    @GetMapping
    public String form(Model model) {

        model.addAttribute("cases", caseRepository.findAll());
        model.addAttribute("officers", policeRepository.findAll());
        model.addAttribute("detainees", detaineeRepository.findAll());
        model.addAttribute("witnesses", witnessRepository.findAll());

        return "cases/assign";
    }

    @PostMapping("/officer")
    public String addOfficer(
            @RequestParam int caseId,
            @RequestParam int officerId
    ) {
        executeInsert("""
            INSERT INTO case_officers(case_id, officer_id)
            VALUES (?, ?)
        """, caseId, officerId);

        return "redirect:/cases/assign";
    }

    @PostMapping("/detainee")
    public String addDetainee(
            @RequestParam int caseId,
            @RequestParam int detaineeId
    ) {
        executeInsert("""
            INSERT INTO case_detainees(case_id, detainee_id)
            VALUES (?, ?)
        """, caseId, detaineeId);

        return "redirect:/cases/assign";
    }

    @PostMapping("/witness")
    public String addWitness(
            @RequestParam int caseId,
            @RequestParam int witnessId
    ) {
        executeInsert("""
            INSERT INTO case_witnesses(case_id, witness_id)
            VALUES (?, ?)
        """, caseId, witnessId);

        return "redirect:/cases/assign";
    }

    private void executeInsert(String sql, int a, int b) {

        try (
                Connection c = dataSource.getConnection();
                PreparedStatement ps = c.prepareStatement(sql)
        ) {

            ps.setInt(1, a);
            ps.setInt(2, b);

            ps.executeUpdate();

        } catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
}