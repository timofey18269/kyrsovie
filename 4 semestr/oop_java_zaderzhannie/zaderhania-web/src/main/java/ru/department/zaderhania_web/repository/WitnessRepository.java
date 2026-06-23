package ru.department.zaderhania_web.repository;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;
import ru.department.zaderhania_web.model.Witness;

import javax.sql.DataSource;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Repository
public class WitnessRepository implements CrudRepository<Witness> {

    private final DataSource dataSource;

    @Autowired
    public WitnessRepository(DataSource dataSource) {
        this.dataSource = dataSource;
    }

    @Override
    public List<Witness> findAll() {

        List<Witness> list = new ArrayList<>();

        String sql = """
                SELECT *
                FROM witnesses
                ORDER BY witness_id
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql);
                ResultSet rs = statement.executeQuery()
        ) {

            while (rs.next()) {
                list.add(mapRow(rs));
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return list;
    }

    @Override
    public Witness findById(int id) {

        String sql = """
                SELECT *
                FROM witnesses
                WHERE witness_id = ?
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setInt(1, id);

            try (ResultSet rs = statement.executeQuery()) {

                if (rs.next()) {
                    return mapRow(rs);
                }
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return null;
    }

    @Override
    public void insert(Witness witness) {

        String sql = """
                INSERT INTO witnesses
                (
                    last_name,
                    first_name,
                    middle_name,
                    phone,
                    address,
                    statement
                )
                VALUES (?, ?, ?, ?, ?, ?)
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            fillStatement(statement, witness);

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void update(Witness witness) {

        String sql = """
                UPDATE witnesses
                SET last_name=?,
                    first_name=?,
                    middle_name=?,
                    phone=?,
                    address=?,
                    statement=?
                WHERE witness_id=?
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            fillStatement(statement, witness);

            statement.setInt(7, witness.getWitnessId());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void delete(int id) {

        String sql = """
                DELETE FROM witnesses
                WHERE witness_id = ?
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

    private Witness mapRow(ResultSet rs) throws SQLException {

        Witness witness = new Witness();

        witness.setWitnessId(rs.getInt("witness_id"));

        witness.setLastName(rs.getString("last_name"));
        witness.setFirstName(rs.getString("first_name"));
        witness.setMiddleName(rs.getString("middle_name"));

        witness.setPhone(rs.getString("phone"));
        witness.setAddress(rs.getString("address"));
        witness.setStatement(rs.getString("statement"));

        return witness;
    }

    private void fillStatement(
            PreparedStatement statement,
            Witness witness
    ) throws SQLException {

        statement.setString(1, witness.getLastName());
        statement.setString(2, witness.getFirstName());
        statement.setString(3, witness.getMiddleName());
        statement.setString(4, witness.getPhone());
        statement.setString(5, witness.getAddress());
        statement.setString(6, witness.getStatement());
    }
}