package ru.department.zaderhania_web.repository;

import ru.department.zaderhania_web.dto.MeasureInfoDto;
import ru.department.zaderhania_web.model.Measure;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

import javax.sql.DataSource;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Repository
public class MeasureRepository implements CrudRepository<Measure> {

    private final DataSource dataSource;

    @Autowired
    public MeasureRepository(DataSource dataSource) {
        this.dataSource = dataSource;
    }

    public List<MeasureInfoDto> findAllDetailed() {

        List<MeasureInfoDto> list = new ArrayList<>();

        String sql = """
            SELECT
                m.measure_id,
                c.case_number,
                CONCAT(
                    d.last_name, ' ',
                    d.first_name, ' ',
                    COALESCE(d.middle_name,'')
                ) detainee_name,
                m.measure_type,
                m.description,
                m.issued_at
            FROM measures m
            JOIN detainees d
                ON d.detainee_id = m.detainee_id
            JOIN cases c
                ON c.case_id = m.case_id
            ORDER BY m.measure_id
            """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql);
                ResultSet rs = statement.executeQuery()
        ) {

            while (rs.next()) {

                MeasureInfoDto dto = new MeasureInfoDto();

                dto.setMeasureId(rs.getInt("measure_id"));
                dto.setCaseNumber(rs.getString("case_number"));
                dto.setDetaineeName(rs.getString("detainee_name"));
                dto.setMeasureType(rs.getString("measure_type"));
                dto.setDescription(rs.getString("description"));
                dto.setIssuedAt(rs.getTimestamp("issued_at"));

                list.add(dto);
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return list;
    }

    @Override
    public List<Measure> findAll() {
        return new ArrayList<>();
    }

    @Override
    public Measure findById(int id) {

        String sql = """
                SELECT *
                FROM measures
                WHERE measure_id = ?
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setInt(1, id);

            try (ResultSet rs = statement.executeQuery()) {

                if (rs.next()) {

                    Measure m = new Measure();

                    m.setMeasureId(rs.getInt("measure_id"));
                    m.setCaseId(rs.getInt("case_id"));
                    m.setDetaineeId(rs.getInt("detainee_id"));
                    m.setMeasureType(rs.getString("measure_type"));
                    m.setDescription(rs.getString("description"));
                    m.setDurationDays((Integer) rs.getObject("duration_days"));
                    m.setIssuedAt(rs.getTimestamp("issued_at"));

                    return m;
                }
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return null;
    }

    @Override
    public void insert(Measure m) {

        String sql = """
            INSERT INTO measures(
                case_id,
                detainee_id,
                measure_type,
                description,
                duration_days
            )
            VALUES (?, ?, ?, ?, ?)
            """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setInt(1, m.getCaseId());
            statement.setInt(2, m.getDetaineeId());
            statement.setString(3, m.getMeasureType());
            statement.setString(4, m.getDescription());

            if (m.getDurationDays() == null)
                statement.setNull(5, Types.INTEGER);
            else
                statement.setInt(5, m.getDurationDays());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void update(Measure m) {

        String sql = """
            UPDATE measures
            SET case_id=?,
                detainee_id=?,
                measure_type=?,
                description=?,
                duration_days=?
            WHERE measure_id=?
            """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setInt(1, m.getCaseId());
            statement.setInt(2, m.getDetaineeId());
            statement.setString(3, m.getMeasureType());
            statement.setString(4, m.getDescription());

            if (m.getDurationDays() == null)
                statement.setNull(5, Types.INTEGER);
            else
                statement.setInt(5, m.getDurationDays());

            statement.setInt(6, m.getMeasureId());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void delete(int id) {

        String sql = """
            DELETE FROM measures
            WHERE measure_id = ?
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