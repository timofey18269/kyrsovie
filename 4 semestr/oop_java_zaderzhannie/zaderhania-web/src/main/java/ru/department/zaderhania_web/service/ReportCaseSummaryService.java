package ru.department.zaderhania_web.service;

import org.springframework.stereotype.Service;
import ru.department.zaderhania_web.dto.ReportCaseSummaryDto;
import ru.department.zaderhania_web.repository.ReportCaseSummaryRepository;

import java.util.List;

@Service
public class ReportCaseSummaryService {

    private final ReportCaseSummaryRepository repository;

    public ReportCaseSummaryService(
            ReportCaseSummaryRepository repository
    ) {
        this.repository = repository;
    }

    public List<ReportCaseSummaryDto> getAll() {
        return repository.findAll();
    }
}