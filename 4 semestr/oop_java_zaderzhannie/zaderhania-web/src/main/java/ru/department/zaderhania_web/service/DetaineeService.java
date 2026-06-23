package ru.department.zaderhania_web.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import ru.department.zaderhania_web.model.Detainee;
import ru.department.zaderhania_web.repository.DetaineeRepository;

import java.util.List;

@Service
public class DetaineeService implements CrudService<Detainee> {

    private final DetaineeRepository repository;

    @Autowired
    public DetaineeService(DetaineeRepository repository) {
        this.repository = repository;
    }

    @Override
    public List<Detainee> getAll() {
        return repository.findAll();
    }

    @Override
    public Detainee getById(int id) {
        return repository.findById(id);
    }

    @Override
    public void create(Detainee detainee) {
        repository.insert(detainee);
    }

    @Override
    public void update(Detainee detainee) {
        repository.update(detainee);
    }

    @Override
    public void delete(int id) {
        repository.delete(id);
    }
}