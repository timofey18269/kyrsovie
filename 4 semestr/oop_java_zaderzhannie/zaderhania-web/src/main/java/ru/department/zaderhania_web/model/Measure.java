package ru.department.zaderhania_web.model;

import java.sql.Timestamp;

public class Measure {

    private int measureId;
    private int caseId;
    private int detaineeId;

    private String measureType;
    private String description;

    private Integer durationDays;
    private Timestamp issuedAt;

    public Measure() {
    }

    public int getMeasureId() {
        return measureId;
    }

    public void setMeasureId(int measureId) {
        this.measureId = measureId;
    }

    public int getCaseId() {
        return caseId;
    }

    public void setCaseId(int caseId) {
        this.caseId = caseId;
    }

    public int getDetaineeId() {
        return detaineeId;
    }

    public void setDetaineeId(int detaineeId) {
        this.detaineeId = detaineeId;
    }

    public String getMeasureType() {
        return measureType;
    }

    public void setMeasureType(String measureType) {
        this.measureType = measureType;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public Integer getDurationDays() {
        return durationDays;
    }

    public void setDurationDays(Integer durationDays) {
        this.durationDays = durationDays;
    }

    public Timestamp getIssuedAt() {
        return issuedAt;
    }

    public void setIssuedAt(Timestamp issuedAt) {
        this.issuedAt = issuedAt;
    }
}