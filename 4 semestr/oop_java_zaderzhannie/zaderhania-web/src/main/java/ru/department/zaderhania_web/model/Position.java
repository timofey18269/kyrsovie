package ru.department.zaderhania_web.model;

public class Position {

    private int positionId;
    private String name;
    private Double salary;

    public Position() {
    }

    public Position(int positionId, String name, Double salary) {
        this.positionId = positionId;
        this.name = name;
        this.salary = salary;
    }

    public int getPositionId() {
        return positionId;
    }

    public void setPositionId(int positionId) {
        this.positionId = positionId;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public Double getSalary() {
        return salary;
    }

    public void setSalary(Double salary) {
        this.salary = salary;
    }
}