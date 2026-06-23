package ru.department.zaderhania_web.service;

import ru.department.zaderhania_web.model.OffenseType;
import ru.department.zaderhania_web.repository.OffenseTypeRepository;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class OffenseTypeService implements CrudService<OffenseType> {

    private final OffenseTypeRepository repository;

    @Autowired
    public OffenseTypeService(OffenseTypeRepository repository) {
        this.repository = repository;
    }

    @Override
    public List<OffenseType> getAll() {
        return repository.findAll();
    }

    @Override
    public OffenseType getById(int id) {
        return repository.findById(id);
    }

    @Override
    public void create(OffenseType t) {
        repository.insert(t);
    }

    @Override
    public void update(OffenseType t) {
        repository.update(t);
    }

    @Override
    public void delete(int id) {
        repository.delete(id);
    }
}