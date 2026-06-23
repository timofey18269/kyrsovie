package ru.department.zaderhania_web.repository;

import ru.department.zaderhania_web.model.OffenseType;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

import javax.sql.DataSource;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Repository
public class OffenseTypeRepository implements CrudRepository<OffenseType> {

    private final DataSource dataSource;

    @Autowired
    public OffenseTypeRepository(DataSource dataSource) {
        this.dataSource = dataSource;
    }

    @Override
    public List<OffenseType> findAll() {

        List<OffenseType> list = new ArrayList<>();

        String sql = """
                SELECT offense_type_id, name, description
                FROM offense_types
                ORDER BY offense_type_id
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql);
                ResultSet rs = statement.executeQuery()
        ) {

            while (rs.next()) {

                OffenseType t = new OffenseType();

                t.setOffenseTypeId(rs.getInt("offense_type_id"));
                t.setName(rs.getString("name"));
                t.setDescription(rs.getString("description"));

                list.add(t);
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return list;
    }

    @Override
    public OffenseType findById(int id) {

        String sql = """
                SELECT offense_type_id, name, description
                FROM offense_types
                WHERE offense_type_id = ?
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setInt(1, id);

            try (ResultSet rs = statement.executeQuery()) {

                if (rs.next()) {

                    OffenseType t = new OffenseType();

                    t.setOffenseTypeId(rs.getInt("offense_type_id"));
                    t.setName(rs.getString("name"));
                    t.setDescription(rs.getString("description"));

                    return t;
                }
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return null;
    }

    @Override
    public void insert(OffenseType t) {

        String sql = """
                INSERT INTO offense_types(name, description)
                VALUES (?, ?)
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setString(1, t.getName());
            statement.setString(2, t.getDescription());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void update(OffenseType t) {

        String sql = """
                UPDATE offense_types
                SET name = ?,
                    description = ?
                WHERE offense_type_id = ?
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setString(1, t.getName());
            statement.setString(2, t.getDescription());
            statement.setInt(3, t.getOffenseTypeId());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void delete(int id) {

        String sql = """
                DELETE FROM offense_types
                WHERE offense_type_id = ?
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setInt(1, id);
            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }
}