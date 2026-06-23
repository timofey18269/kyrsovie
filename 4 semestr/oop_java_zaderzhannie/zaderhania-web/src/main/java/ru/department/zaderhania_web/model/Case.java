package ru.department.zaderhania_web.model;

import java.time.LocalDateTime;

public class Case {

    private int caseId;
    private String caseNumber;

    private Integer reportId;
    private Integer offenseTypeId;

    private LocalDateTime openedAt;
    private LocalDateTime  closedAt;

    private String status;
    private String description;

    public Case() {
    }

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

    public Integer getReportId() {
        return reportId;
    }

    public void setReportId(Integer reportId) {
        this.reportId = reportId;
    }

    public Integer getOffenseTypeId() {
        return offenseTypeId;
    }

    public void setOffenseTypeId(Integer offenseTypeId) {
        this.offenseTypeId = offenseTypeId;
    }

    public LocalDateTime  getOpenedAt() {
        return openedAt;
    }

    public void setOpenedAt(LocalDateTime  openedAt) {
        this.openedAt = openedAt;
    }

    public LocalDateTime  getClosedAt() {
        return closedAt;
    }

    public void setClosedAt(LocalDateTime  closedAt) {
        this.closedAt = closedAt;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }
}