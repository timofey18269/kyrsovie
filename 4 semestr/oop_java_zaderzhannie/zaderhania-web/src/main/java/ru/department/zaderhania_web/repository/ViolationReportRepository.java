package ru.department.zaderhania_web.repository;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;
import ru.department.zaderhania_web.model.ViolationReport;

import javax.sql.DataSource;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Repository
public class ViolationReportRepository implements CrudRepository<ViolationReport> {

    private final DataSource dataSource;

    @Autowired
    public ViolationReportRepository(DataSource dataSource) {
        this.dataSource = dataSource;
    }

    @Override
    public List<ViolationReport> findAll() {

        List<ViolationReport> list = new ArrayList<>();

        String sql = """
            SELECT vr.*,

                   CONCAT(
                       w.last_name,' ',
                       w.first_name,' ',
                       COALESCE(w.middle_name,'')
                   ) AS witness_name,

                   ot.name AS offense_type_name

            FROM violation_reports vr

            LEFT JOIN witnesses w
                ON vr.witness_id = w.witness_id

            LEFT JOIN offense_types ot
                ON vr.offense_type_id = ot.offense_type_id

            ORDER BY vr.report_id
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
    public ViolationReport findById(int id) {

        String sql = """
            SELECT vr.*,

                   CONCAT(
                       w.last_name,' ',
                       w.first_name,' ',
                       COALESCE(w.middle_name,'')
                   ) AS witness_name,

                   ot.name AS offense_type_name

            FROM violation_reports vr

            LEFT JOIN witnesses w
                ON vr.witness_id = w.witness_id

            LEFT JOIN offense_types ot
                ON vr.offense_type_id = ot.offense_type_id

            WHERE vr.report_id = ?
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
    public void insert(ViolationReport report) {

        String sql = """
            INSERT INTO violation_reports
            (
                report_number,
                witness_id,
                offense_type_id,
                location,
                description
            )
            VALUES (?, ?, ?, ?, ?)
            """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            fillStatement(statement, report);

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void update(ViolationReport report) {

        String sql = """
            UPDATE violation_reports
            SET report_number=?,
                witness_id=?,
                offense_type_id=?,
                location=?,
                description=?
            WHERE report_id=?
            """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            fillStatement(statement, report);

            statement.setInt(6, report.getReportId());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void delete(int id) {

        String sql = """
            DELETE FROM violation_reports
            WHERE report_id = ?
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

    private ViolationReport mapRow(ResultSet rs) throws SQLException {

        ViolationReport report = new ViolationReport();

        report.setReportId(rs.getInt("report_id"));
        report.setReportNumber(rs.getString("report_number"));

        Timestamp timestamp = rs.getTimestamp("report_date");
        if (timestamp != null) {
            report.setReportDate(timestamp.toLocalDateTime());
        }

        report.setWitnessId((Integer) rs.getObject("witness_id"));
        report.setOffenseTypeId((Integer) rs.getObject("offense_type_id"));

        report.setWitnessName(rs.getString("witness_name"));
        report.setOffenseTypeName(rs.getString("offense_type_name"));

        report.setLocation(rs.getString("location"));
        report.setDescription(rs.getString("description"));

        return report;
    }

    private void fillStatement(
            PreparedStatement statement,
            ViolationReport report
    ) throws SQLException {

        statement.setString(1, report.getReportNumber());

        if (report.getWitnessId() == null)
            statement.setNull(2, Types.INTEGER);
        else
            statement.setInt(2, report.getWitnessId());

        if (report.getOffenseTypeId() == null)
            statement.setNull(3, Types.INTEGER);
        else
            statement.setInt(3, report.getOffenseTypeId());

        statement.setString(4, report.getLocation());
        statement.setString(5, report.getDescription());
    }
}