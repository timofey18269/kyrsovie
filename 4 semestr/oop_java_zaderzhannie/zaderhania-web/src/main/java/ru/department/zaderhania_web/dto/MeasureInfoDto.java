package ru.department.zaderhania_web.dto;

import java.sql.Timestamp;

public class MeasureInfoDto {

    private int measureId;

    private String caseNumber;
    private String detaineeName;

    private String measureType;
    private String description;

    private Timestamp issuedAt;

    public MeasureInfoDto() {
    }

    public int getMeasureId() {
        return measureId;
    }

    public void setMeasureId(int measureId) {
        this.measureId = measureId;
    }

    public String getCaseNumber() {
        return caseNumber;
    }

    public void setCaseNumber(String caseNumber) {
        this.caseNumber = caseNumber;
    }

    public String getDetaineeName() {
        return detaineeName;
    }

    public void setDetaineeName(String detaineeName) {
        this.detaineeName = detaineeName;
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

    public Timestamp getIssuedAt() {
        return issuedAt;
    }

    public void setIssuedAt(Timestamp issuedAt) {
        this.issuedAt = issuedAt;
    }
}