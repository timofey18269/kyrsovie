package ru.department.zaderhania_web.service;

import org.springframework.stereotype.Service;
import ru.department.zaderhania_web.dto.ReportOfficerWorkloadDto;
import ru.department.zaderhania_web.repository.ReportOfficerWorkloadRepository;

import java.util.List;

@Service
public class ReportOfficerWorkloadService {

    private final ReportOfficerWorkloadRepository repository;

    public ReportOfficerWorkloadService(
            ReportOfficerWorkloadRepository repository
    ) {
        this.repository = repository;
    }

    public List<ReportOfficerWorkloadDto> getAll() {
        return repository.findAll();
    }
}