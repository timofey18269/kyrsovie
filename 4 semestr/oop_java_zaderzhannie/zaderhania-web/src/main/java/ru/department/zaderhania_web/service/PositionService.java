package ru.department.zaderhania_web.service;

import ru.department.zaderhania_web.model.Position;
import ru.department.zaderhania_web.repository.PositionRepository;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class PositionService implements CrudService<Position> {

    private final PositionRepository repository;

    @Autowired
    public PositionService(PositionRepository repository) {
        this.repository = repository;
    }

    @Override
    public List<Position> getAll() {
        return repository.findAll();
    }

    @Override
    public Position getById(int id) {
        return repository.findById(id);
    }

    @Override
    public void create(Position position) {
        repository.insert(position);
    }

    @Override
    public void update(Position position) {
        repository.update(position);
    }

    @Override
    public void delete(int id) {
        repository.delete(id);
    }
}