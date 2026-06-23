package ru.department.zaderhania_web.service;

import java.util.List;

public interface CrudService<T> {

    List<T> getAll();

    T getById(int id);

    void create(T entity);

    void update(T entity);

    void delete(int id);
}