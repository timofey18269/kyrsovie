package ru.department.zaderhania_web.repository;

import java.util.List;

public interface CrudRepository<T> {

    List<T> findAll();

    T findById(int id);

    void insert(T entity);

    void update(T entity);

    void delete(int id);
}