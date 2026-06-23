package ru.department.zaderhania_web.model;

public class OffenseType {

    private int offenseTypeId;
    private String name;
    private String description;

    public OffenseType() {
    }

    public OffenseType(int offenseTypeId, String name, String description) {
        this.offenseTypeId = offenseTypeId;
        this.name = name;
        this.description = description;
    }

    public int getOffenseTypeId() {
        return offenseTypeId;
    }

    public void setOffenseTypeId(int offenseTypeId) {
        this.offenseTypeId = offenseTypeId;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }
}