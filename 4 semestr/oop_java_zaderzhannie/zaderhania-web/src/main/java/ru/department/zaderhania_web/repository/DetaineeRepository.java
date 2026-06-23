package ru.department.zaderhania_web.repository;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;
import ru.department.zaderhania_web.model.Detainee;

import javax.sql.DataSource;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Repository
public class DetaineeRepository implements CrudRepository<Detainee> {

    private final DataSource dataSource;

    @Autowired
    public DetaineeRepository(DataSource dataSource) {
        this.dataSource = dataSource;
    }

    @Override
    public List<Detainee> findAll() {

        List<Detainee> list = new ArrayList<>();

        String sql = """
                SELECT *
                FROM detainees
                ORDER BY detainee_id
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql);
                ResultSet rs = statement.executeQuery()
        ) {

            while (rs.next()) {

                Detainee d = mapRow(rs);

                list.add(d);
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return list;
    }

    @Override
    public Detainee findById(int id) {

        String sql = """
                SELECT *
                FROM detainees
                WHERE detainee_id = ?
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
    public void insert(Detainee d) {

        String sql = """
                INSERT INTO detainees
                (
                    last_name,
                    first_name,
                    middle_name,
                    birth_date,
                    passport_number,
                    address,
                    phone,
                    status,
                    notes
                )
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            fillStatement(statement, d);

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void update(Detainee d) {

        String sql = """
                UPDATE detainees
                SET last_name=?,
                    first_name=?,
                    middle_name=?,
                    birth_date=?,
                    passport_number=?,
                    address=?,
                    phone=?,
                    status=?,
                    notes=?
                WHERE detainee_id=?
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            fillStatement(statement, d);

            statement.setInt(10, d.getDetaineeId());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void delete(int id) {

        String sql = """
                DELETE FROM detainees
                WHERE detainee_id = ?
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

    private Detainee mapRow(ResultSet rs) throws SQLException {

        Detainee d = new Detainee();

        d.setDetaineeId(rs.getInt("detainee_id"));

        d.setLastName(rs.getString("last_name"));
        d.setFirstName(rs.getString("first_name"));
        d.setMiddleName(rs.getString("middle_name"));

        Date birthDate = rs.getDate("birth_date");
        if (birthDate != null) {
            d.setBirthDate(birthDate.toLocalDate());
        }

        d.setPassportNumber(rs.getString("passport_number"));
        d.setAddress(rs.getString("address"));
        d.setPhone(rs.getString("phone"));
        d.setStatus(rs.getString("status"));
        d.setNotes(rs.getString("notes"));

        Timestamp created = rs.getTimestamp("created_at");
        if (created != null) {
            d.setCreatedAt(created.toLocalDateTime());
        }

        return d;
    }

    private void fillStatement(
            PreparedStatement statement,
            Detainee d
    ) throws SQLException {

        statement.setString(1, d.getLastName());
        statement.setString(2, d.getFirstName());
        statement.setString(3, d.getMiddleName());

        if (d.getBirthDate() == null)
            statement.setNull(4, Types.DATE);
        else
            statement.setDate(4, Date.valueOf(d.getBirthDate()));

        statement.setString(5, d.getPassportNumber());
        statement.setString(6, d.getAddress());
        statement.setString(7, d.getPhone());
        statement.setString(8, d.getStatus());
        statement.setString(9, d.getNotes());
    }
}