package ru.department.zaderhania_web.model;

import java.time.LocalDate;
import java.time.LocalDateTime;

public class Detainee extends Person {

    private int detaineeId;
    private LocalDate birthDate;
    private String passportNumber;
    private String address;
    private String status;
    private String notes;
    private LocalDateTime createdAt;

    public Detainee() {
        super();
    }

    public int getDetaineeId() {
        return detaineeId;
    }

    public void setDetaineeId(int detaineeId) {
        this.detaineeId = detaineeId;
    }

    public LocalDate getBirthDate() {
        return birthDate;
    }

    public void setBirthDate(LocalDate birthDate) {
        this.birthDate = birthDate;
    }

    public String getPassportNumber() {
        return passportNumber;
    }

    public void setPassportNumber(String passportNumber) {
        this.passportNumber = passportNumber;
    }

    public String getAddress() {
        return address;
    }

    public void setAddress(String address) {
        this.address = address;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getNotes() {
        return notes;
    }

    public void setNotes(String notes) {
        this.notes = notes;
    }

    public LocalDateTime getCreatedAt() {
        return createdAt;
    }

    public void setCreatedAt(LocalDateTime createdAt) {
        this.createdAt = createdAt;
    }
}