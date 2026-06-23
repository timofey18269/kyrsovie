package ru.department.zaderhania_web.service;

import ru.department.zaderhania_web.dto.CaseDetailsDto;
import ru.department.zaderhania_web.dto.CaseListDto;
import ru.department.zaderhania_web.model.Case;
import ru.department.zaderhania_web.repository.CaseRepository;

import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class CaseService implements CrudService<Case> {

    private final CaseRepository repository;

    public CaseService(CaseRepository repository) {
        this.repository = repository;
    }

    public List<CaseListDto> getAllDetailed() {
        return repository.findAllDetailed();
    }

    public CaseDetailsDto getDetails(int id) {

        CaseDetailsDto dto = new CaseDetailsDto();

        dto.setCaseInfo(repository.findById(id));
        dto.setOfficers(repository.getOfficers(id));
        dto.setDetainees(repository.getDetainees(id));
        dto.setWitnesses(repository.getWitnesses(id));
        dto.setMeasures(repository.getMeasures(id));

        return dto;
    }

    @Override
    public List<Case> getAll() {
        return repository.findAll();
    }

    @Override
    public Case getById(int id) {
        return repository.findById(id);
    }

    @Override
    public void create(Case entity) {
        repository.insert(entity);
    }

    @Override
    public void update(Case entity) {
        repository.update(entity);
    }

    @Override
    public void delete(int id) {
        repository.delete(id);
    }
}