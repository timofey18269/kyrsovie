package ru.department.zaderhania_web.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import ru.department.zaderhania_web.model.ViolationReport;
import ru.department.zaderhania_web.repository.ViolationReportRepository;

import java.util.List;

@Service
public class ViolationReportService implements CrudService<ViolationReport> {

    private final ViolationReportRepository repository;

    @Autowired
    public ViolationReportService(
            ViolationReportRepository repository
    ) {
        this.repository = repository;
    }

    @Override
    public List<ViolationReport> getAll() {
        return repository.findAll();
    }

    @Override
    public ViolationReport getById(int id) {
        return repository.findById(id);
    }

    @Override
    public void create(ViolationReport report) {
        repository.insert(report);
    }

    @Override
    public void update(ViolationReport report) {
        repository.update(report);
    }

    @Override
    public void delete(int id) {
        repository.delete(id);
    }
}