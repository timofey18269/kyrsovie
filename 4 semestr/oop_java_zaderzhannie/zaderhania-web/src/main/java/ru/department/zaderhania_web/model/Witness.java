package ru.department.zaderhania_web.model;

public class Witness extends Person {

    private int witnessId;
    private String address;
    private String statement;

    public Witness() {
        super();
    }

    public int getWitnessId() {
        return witnessId;
    }

    public void setWitnessId(int witnessId) {
        this.witnessId = witnessId;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getStatement() {
        return statement;
    }

    public void setStatement(String statement) {
        this.statement = statement;
    }
}