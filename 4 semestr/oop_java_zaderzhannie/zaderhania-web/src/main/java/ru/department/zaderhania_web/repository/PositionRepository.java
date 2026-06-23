package ru.department.zaderhania_web.repository;

import ru.department.zaderhania_web.model.Position;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

import javax.sql.DataSource;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Repository
public class PositionRepository implements CrudRepository<Position> {

    private final DataSource dataSource;

    @Autowired
    public PositionRepository(DataSource dataSource) {
        this.dataSource = dataSource;
    }

    @Override
    public List<Position> findAll() {

        List<Position> positions = new ArrayList<>();

        String sql = """
                SELECT position_id, name, salary
                FROM positions
                ORDER BY position_id
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql);
                ResultSet rs = statement.executeQuery()
        ) {

            while (rs.next()) {

                Position position = new Position();

                position.setPositionId(rs.getInt("position_id"));
                position.setName(rs.getString("name"));
                position.setSalary(rs.getDouble("salary"));

                positions.add(position);
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return positions;
    }

    @Override
    public Position findById(int id) {

        String sql = """
                SELECT position_id, name, salary
                FROM positions
                WHERE position_id = ?
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setInt(1, id);

            try (ResultSet rs = statement.executeQuery()) {

                if (rs.next()) {

                    Position position = new Position();

                    position.setPositionId(rs.getInt("position_id"));
                    position.setName(rs.getString("name"));
                    position.setSalary(rs.getDouble("salary"));

                    return position;
                }
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return null;
    }

    @Override
    public void insert(Position position) {

        String sql = """
                INSERT INTO positions(name, salary)
                VALUES (?, ?)
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setString(1, position.getName());
            statement.setDouble(2, position.getSalary());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void update(Position position) {

        String sql = """
                UPDATE positions
                SET name = ?,
                    salary = ?
                WHERE position_id = ?
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setString(1, position.getName());
            statement.setDouble(2, position.getSalary());
            statement.setInt(3, position.getPositionId());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void delete(int id) {

        String sql = """
                DELETE FROM positions
                WHERE position_id = ?
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