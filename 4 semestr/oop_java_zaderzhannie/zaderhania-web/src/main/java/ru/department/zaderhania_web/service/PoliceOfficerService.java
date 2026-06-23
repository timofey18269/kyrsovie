package ru.department.zaderhania_web.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import ru.department.zaderhania_web.model.PoliceOfficer;
import ru.department.zaderhania_web.repository.PoliceOfficerRepository;

import java.util.List;

@Service
public class PoliceOfficerService implements CrudService<PoliceOfficer> {

    private final PoliceOfficerRepository repository;

    @Autowired
    public PoliceOfficerService(PoliceOfficerRepository repository) {
        this.repository = repository;
    }

    @Override
    public List<PoliceOfficer> getAll() {
        return repository.findAll();
    }

    @Override
    public PoliceOfficer getById(int id) {
        return repository.findById(id);
    }

    @Override
    public void create(PoliceOfficer officer) {
        repository.insert(officer);
    }

    @Override
    public void update(PoliceOfficer officer) {
        repository.update(officer);
    }

    @Override
    public void delete(int id) {
        repository.delete(id);
    }
}