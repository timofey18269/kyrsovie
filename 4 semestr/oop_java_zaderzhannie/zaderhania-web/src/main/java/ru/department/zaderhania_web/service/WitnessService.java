package ru.department.zaderhania_web.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import ru.department.zaderhania_web.model.Witness;
import ru.department.zaderhania_web.repository.WitnessRepository;

import java.util.List;

@Service
public class WitnessService implements CrudService<Witness> {

    private final WitnessRepository repository;

    @Autowired
    public WitnessService(WitnessRepository repository) {
        this.repository = repository;
    }

    @Override
    public List<Witness> getAll() {
        return repository.findAll();
    }

    @Override
    public Witness getById(int id) {
        return repository.findById(id);
    }

    @Override
    public void create(Witness witness) {
        repository.insert(witness);
    }

    @Override
    public void update(Witness witness) {
        repository.update(witness);
    }

    @Override
    public void delete(int id) {
        repository.delete(id);
    }
}