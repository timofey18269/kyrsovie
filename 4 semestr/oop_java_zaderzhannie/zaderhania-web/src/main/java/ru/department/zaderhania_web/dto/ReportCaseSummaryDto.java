package ru.department.zaderhania_web.dto;

import java.time.LocalDateTime;

public class ReportCaseSummaryDto {

    private int caseId;
    private String caseNumber;

    private String offenseType;
    private String caseStatus;

    private String reportNumber;

    private LocalDateTime openedAt;
    private LocalDateTime closedAt;

    private int officersCount;
    private int detaineesCount;
    private int witnessesCount;

    private int measuresCount;

    private String measureTypes;

    private String description;

    public int getCaseId() {
        return caseId;
    }

    public void setCaseId(int caseId) {
        this.caseId = caseId;
    }

    public String getCaseNumber() {
        return caseNumber;
    }

    public void setCaseNumber(String caseNumber) {
        this.caseNumber = caseNumber;
    }

    public String getOffenseType() {
        return offenseType;
    }

    public void setOffenseType(String offenseType) {
        this.offenseType = offenseType;
    }

    public String getCaseStatus() {
        return caseStatus;
    }

    public void setCaseStatus(String caseStatus) {
        this.caseStatus = caseStatus;
    }

    public String getReportNumber() {
        return reportNumber;
    }

    public void setReportNumber(String reportNumber) {
        this.reportNumber = reportNumber;
    }

    public LocalDateTime getOpenedAt() {
        return openedAt;
    }

    public void setOpenedAt(LocalDateTime openedAt) {
        this.openedAt = openedAt;
    }

    public LocalDateTime getClosedAt() {
        return closedAt;
    }

    public void setClosedAt(LocalDateTime closedAt) {
        this.closedAt = closedAt;
    }

    public int getOfficersCount() {
        return officersCount;
    }

    public void setOfficersCount(int officersCount) {
        this.officersCount = officersCount;
    }

    public int getDetaineesCount() {
        return detaineesCount;
    }

    public void setDetaineesCount(int detaineesCount) {
        this.detaineesCount = detaineesCount;
    }

    public int getWitnessesCount() {
        return witnessesCount;
    }

    public void setWitnessesCount(int witnessesCount) {
        this.witnessesCount = witnessesCount;
    }

    public int getMeasuresCount() {
        return measuresCount;
    }

    public void setMeasuresCount(int measuresCount) {
        this.measuresCount = measuresCount;
    }

    public String getMeasureTypes() {
        return measureTypes;
    }

    public void setMeasureTypes(String measureTypes) {
        this.measureTypes = measureTypes;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }
}