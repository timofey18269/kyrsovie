package ru.department.zaderhania_web.model;

import java.time.LocalDateTime;

public class ViolationReport {

    private int reportId;

    private String reportNumber;
    private LocalDateTime reportDate;

    private Integer witnessId;
    private Integer offenseTypeId;

    private String witnessName;
    private String offenseTypeName;

    private String location;
    private String description;

    public ViolationReport() {
    }

    public int getReportId() {
        return reportId;
    }

    public void setReportId(int reportId) {
        this.reportId = reportId;
    }

    public String getReportNumber() {
        return reportNumber;
    }

    public void setReportNumber(String reportNumber) {
        this.reportNumber = reportNumber;
    }

    public LocalDateTime getReportDate() {
        return reportDate;
    }

    public void setReportDate(LocalDateTime reportDate) {
        this.reportDate = reportDate;
    }

    public Integer getWitnessId() {
        return witnessId;
    }

    public void setWitnessId(Integer witnessId) {
        this.witnessId = witnessId;
    }

    public Integer getOffenseTypeId() {
        return offenseTypeId;
    }

    public void setOffenseTypeId(Integer offenseTypeId) {
        this.offenseTypeId = offenseTypeId;
    }

    public String getWitnessName() {
        return witnessName;
    }

    public void setWitnessName(String witnessName) {
        this.witnessName = witnessName;
    }

    public String getOffenseTypeName() {
        return offenseTypeName;
    }

    public void setOffenseTypeName(String offenseTypeName) {
        this.offenseTypeName = offenseTypeName;
    }

    public String getLocation() {
        return location;
    }

    public void setLocation(String location) {
        this.location = location;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }
}