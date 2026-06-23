package ru.department.zaderhania_web.dto;

public class ReportOfficerWorkloadDto {

    private int officerId;

    private String badgeNumber;
    private String fullName;

    private String positionName;
    private String rankName;

    private boolean active;

    private int casesCount;
    private String roles;

    private int detaineesHandled;
    private int measuresInCases;

    public int getOfficerId() {
        return officerId;
    }

    public void setOfficerId(int officerId) {
        this.officerId = officerId;
    }

    public String getBadgeNumber() {
        return badgeNumber;
    }

    public void setBadgeNumber(String badgeNumber) {
        this.badgeNumber = badgeNumber;
    }

    public String getFullName() {
        return fullName;
    }

    public void setFullName(String fullName) {
        this.fullName = fullName;
    }

    public String getPositionName() {
        return positionName;
    }

    public void setPositionName(String positionName) {
        this.positionName = positionName;
    }

    public String getRankName() {
        return rankName;
    }

    public void setRankName(String rankName) {
        this.rankName = rankName;
    }

    public boolean isActive() {
        return active;
    }

    public void setActive(boolean active) {
        this.active = active;
    }

    public int getCasesCount() {
        return casesCount;
    }

    public void setCasesCount(int casesCount) {
        this.casesCount = casesCount;
    }

    public String getRoles() {
        return roles;
    }

    public void setRoles(String roles) {
        this.roles = roles;
    }

    public int getDetaineesHandled() {
        return detaineesHandled;
    }

    public void setDetaineesHandled(int detaineesHandled) {
        this.detaineesHandled = detaineesHandled;
    }

    public int getMeasuresInCases() {
        return measuresInCases;
    }

    public void setMeasuresInCases(int measuresInCases) {
        this.measuresInCases = measuresInCases;
    }
}