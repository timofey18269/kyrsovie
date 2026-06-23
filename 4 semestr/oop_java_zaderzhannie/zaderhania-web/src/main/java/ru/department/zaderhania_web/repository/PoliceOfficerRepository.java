package ru.department.zaderhania_web.repository;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;
import ru.department.zaderhania_web.model.PoliceOfficer;

import javax.sql.DataSource;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Repository
public class PoliceOfficerRepository implements CrudRepository<PoliceOfficer> {

    private final DataSource dataSource;

    @Autowired
    public PoliceOfficerRepository(DataSource dataSource) {
        this.dataSource = dataSource;
    }

    @Override
    public List<PoliceOfficer> findAll() {

        List<PoliceOfficer> list = new ArrayList<>();

        String sql = """
                SELECT *
                FROM police_officers
                ORDER BY officer_id
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql);
                ResultSet rs = statement.executeQuery()
        ) {

            while (rs.next()) {

                PoliceOfficer officer = new PoliceOfficer();

                officer.setOfficerId(rs.getInt("officer_id"));
                officer.setBadgeNumber(rs.getString("badge_number"));
                officer.setLastName(rs.getString("last_name"));
                officer.setFirstName(rs.getString("first_name"));
                officer.setMiddleName(rs.getString("middle_name"));
                officer.setPositionId((Integer) rs.getObject("position_id"));
                officer.setRankName(rs.getString("rank_name"));

                Date hireDate = rs.getDate("hire_date");
                if (hireDate != null) {
                    officer.setHireDate(hireDate.toLocalDate());
                }

                officer.setPhone(rs.getString("phone"));
                officer.setEmail(rs.getString("email"));
                officer.setActive(rs.getBoolean("active"));

                list.add(officer);
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return list;
    }

    @Override
    public PoliceOfficer findById(int id) {

        String sql = """
                SELECT *
                FROM police_officers
                WHERE officer_id = ?
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setInt(1, id);

            try (ResultSet rs = statement.executeQuery()) {

                if (rs.next()) {

                    PoliceOfficer officer = new PoliceOfficer();

                    officer.setOfficerId(rs.getInt("officer_id"));
                    officer.setBadgeNumber(rs.getString("badge_number"));
                    officer.setLastName(rs.getString("last_name"));
                    officer.setFirstName(rs.getString("first_name"));
                    officer.setMiddleName(rs.getString("middle_name"));
                    officer.setPositionId((Integer) rs.getObject("position_id"));
                    officer.setRankName(rs.getString("rank_name"));

                    Date hireDate = rs.getDate("hire_date");
                    if (hireDate != null) {
                        officer.setHireDate(hireDate.toLocalDate());
                    }

                    officer.setPhone(rs.getString("phone"));
                    officer.setEmail(rs.getString("email"));
                    officer.setActive(rs.getBoolean("active"));

                    return officer;
                }
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return null;
    }

    @Override
    public void insert(PoliceOfficer officer) {

        String sql = """
                INSERT INTO police_officers
                (
                    badge_number,
                    last_name,
                    first_name,
                    middle_name,
                    position_id,
                    rank_name,
                    hire_date,
                    phone,
                    email,
                    active
                )
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setString(1, officer.getBadgeNumber());
            statement.setString(2, officer.getLastName());
            statement.setString(3, officer.getFirstName());
            statement.setString(4, officer.getMiddleName());

            if (officer.getPositionId() == null)
                statement.setNull(5, Types.INTEGER);
            else
                statement.setInt(5, officer.getPositionId());

            statement.setString(6, officer.getRankName());

            if (officer.getHireDate() == null)
                statement.setNull(7, Types.DATE);
            else
                statement.setDate(7, Date.valueOf(officer.getHireDate()));

            statement.setString(8, officer.getPhone());
            statement.setString(9, officer.getEmail());
            statement.setBoolean(10, officer.isActive());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void update(PoliceOfficer officer) {

        String sql = """
                UPDATE police_officers
                SET badge_number=?,
                    last_name=?,
                    first_name=?,
                    middle_name=?,
                    position_id=?,
                    rank_name=?,
                    hire_date=?,
                    phone=?,
                    email=?,
                    active=?
                WHERE officer_id=?
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setString(1, officer.getBadgeNumber());
            statement.setString(2, officer.getLastName());
            statement.setString(3, officer.getFirstName());
            statement.setString(4, officer.getMiddleName());

            if (officer.getPositionId() == null)
                statement.setNull(5, Types.INTEGER);
            else
                statement.setInt(5, officer.getPositionId());

            statement.setString(6, officer.getRankName());

            if (officer.getHireDate() == null)
                statement.setNull(7, Types.DATE);
            else
                statement.setDate(7, Date.valueOf(officer.getHireDate()));

            statement.setString(8, officer.getPhone());
            statement.setString(9, officer.getEmail());
            statement.setBoolean(10, officer.isActive());

            statement.setInt(11, officer.getOfficerId());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void delete(int id) {

        String sql = """
                DELETE FROM police_officers
                WHERE officer_id = ?
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