package ru.department.zaderhania_web.dto;

import ru.department.zaderhania_web.model.Case;

import java.util.List;

public class CaseDetailsDto {

    private Case caseInfo;

    private List<String> officers;
    private List<String> detainees;
    private List<String> witnesses;
    private List<String> measures;

    public Case getCaseInfo() {
        return caseInfo;
    }

    public void setCaseInfo(Case caseInfo) {
        this.caseInfo = caseInfo;
    }

    public List<String> getOfficers() {
        return officers;
    }

    public void setOfficers(List<String> officers) {
        this.officers = officers;
    }

    public List<String> getDetainees() {
        return detainees;
    }

    public void setDetainees(List<String> detainees) {
        this.detainees = detainees;
    }

    public List<String> getWitnesses() {
        return witnesses;
    }

    public void setWitnesses(List<String> witnesses) {
        this.witnesses = witnesses;
    }

    public List<String> getMeasures() {
        return measures;
    }

    public void setMeasures(List<String> measures) {
        this.measures = measures;
    }
}