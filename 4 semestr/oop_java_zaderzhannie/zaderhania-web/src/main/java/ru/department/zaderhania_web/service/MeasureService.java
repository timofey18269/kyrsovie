package ru.department.zaderhania_web.service;

import ru.department.zaderhania_web.dto.MeasureInfoDto;
import ru.department.zaderhania_web.model.Measure;
import ru.department.zaderhania_web.repository.MeasureRepository;

import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class MeasureService implements CrudService<Measure> {

    private final MeasureRepository repository;

    public MeasureService(MeasureRepository repository) {
        this.repository = repository;
    }

    public List<MeasureInfoDto> getAllDetailed() {
        return repository.findAllDetailed();
    }

    @Override
    public List<Measure> getAll() {
        return repository.findAll();
    }

    @Override
    public Measure getById(int id) {
        return repository.findById(id);
    }

    @Override
    public void create(Measure entity) {
        repository.insert(entity);
    }

    @Override
    public void update(Measure entity) {
        repository.update(entity);
    }

    @Override
    public void delete(int id) {
        repository.delete(id);
    }
}