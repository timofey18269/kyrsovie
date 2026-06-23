package ru.department.zaderhania_web.repository;

import ru.department.zaderhania_web.dto.CaseDetailsDto;
import ru.department.zaderhania_web.dto.CaseListDto;
import ru.department.zaderhania_web.model.Case;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;

import javax.sql.DataSource;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Repository
public class CaseRepository implements CrudRepository<Case> {

    private final DataSource dataSource;

    @Autowired
    public CaseRepository(DataSource dataSource) {
        this.dataSource = dataSource;
    }

    public List<CaseListDto> findAllDetailed() {

        List<CaseListDto> result = new ArrayList<>();

        String sql = """
            SELECT
                c.case_id,
                c.case_number,
                vr.report_number,
                ot.name offense_type_name,
                c.opened_at,
                c.closed_at,
                c.status,
                c.description
            FROM cases c
            LEFT JOIN violation_reports vr
                ON vr.report_id = c.report_id
            LEFT JOIN offense_types ot
                ON ot.offense_type_id = c.offense_type_id
            ORDER BY c.case_id
            """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql);
                ResultSet rs = statement.executeQuery()
        ) {

            while (rs.next()) {

                CaseListDto dto = new CaseListDto();

                dto.setCaseId(rs.getInt("case_id"));
                dto.setCaseNumber(rs.getString("case_number"));
                dto.setReportNumber(rs.getString("report_number"));
                dto.setOffenseTypeName(rs.getString("offense_type_name"));
                dto.setOpenedAt(rs.getTimestamp("opened_at"));
                dto.setClosedAt(rs.getTimestamp("closed_at"));
                dto.setStatus(rs.getString("status"));
                dto.setDescription(rs.getString("description"));

                result.add(dto);
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return result;
    }

    @Override
    public List<Case> findAll() {

        List<Case> result = new ArrayList<>();

        String sql = """
            SELECT *
            FROM cases
            ORDER BY case_id
            """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql);
                ResultSet rs = statement.executeQuery()
        ) {

            while (rs.next()) {

                Case c = new Case();

                c.setCaseId(rs.getInt("case_id"));
                c.setCaseNumber(rs.getString("case_number"));

                c.setReportId((Integer) rs.getObject("report_id"));
                c.setOffenseTypeId((Integer) rs.getObject("offense_type_id"));

                Timestamp openedTs = rs.getTimestamp("opened_at");
                Timestamp closedTs = rs.getTimestamp("closed_at");

                if (openedTs != null) {
                    c.setOpenedAt(openedTs.toLocalDateTime());
                }

                if (closedTs != null) {
                    c.setClosedAt(closedTs.toLocalDateTime());
                }

                c.setStatus(rs.getString("status"));
                c.setDescription(rs.getString("description"));

                result.add(c);
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return result;
    }

    @Override
    public Case findById(int id) {

        String sql = """
            SELECT *
            FROM cases
            WHERE case_id = ?
            """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setInt(1, id);

            try (ResultSet rs = statement.executeQuery()) {

                if (rs.next()) {

                    Case c = new Case();

                    c.setCaseId(rs.getInt("case_id"));
                    c.setCaseNumber(rs.getString("case_number"));

                    c.setReportId((Integer) rs.getObject("report_id"));
                    c.setOffenseTypeId((Integer) rs.getObject("offense_type_id"));

                    Timestamp openedTs = rs.getTimestamp("opened_at");
                    Timestamp closedTs = rs.getTimestamp("closed_at");

                    if (openedTs != null) {
                        c.setOpenedAt(openedTs.toLocalDateTime());
                    }

                    if (closedTs != null) {
                        c.setClosedAt(closedTs.toLocalDateTime());
                    }

                    c.setStatus(rs.getString("status"));
                    c.setDescription(rs.getString("description"));

                    return c;
                }
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return null;
    }

    @Override
    public void insert(Case c) {

        String sql = """
            INSERT INTO cases(
                case_number,
                report_id,
                offense_type_id,
                closed_at,
                status,
                description
            )
            VALUES (?, ?, ?, ?, ?, ?)
            """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setString(1, c.getCaseNumber());

            if (c.getReportId() == null)
                statement.setNull(2, Types.INTEGER);
            else
                statement.setInt(2, c.getReportId());

            if (c.getOffenseTypeId() == null)
                statement.setNull(3, Types.INTEGER);
            else
                statement.setInt(3, c.getOffenseTypeId());

            if (c.getClosedAt() == null) {
                statement.setNull(4, Types.TIMESTAMP);
            } else {
                statement.setTimestamp(
                        4,
                        Timestamp.valueOf(c.getClosedAt())
                );
            }
            statement.setString(5, c.getStatus());
            statement.setString(6, c.getDescription());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void update(Case c) {

        String sql = """
            UPDATE cases
            SET case_number=?,
                report_id=?,
                offense_type_id=?,
                closed_at=?,
                status=?,
                description=?
            WHERE case_id=?
            """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setString(1, c.getCaseNumber());

            if (c.getReportId() == null)
                statement.setNull(2, Types.INTEGER);
            else
                statement.setInt(2, c.getReportId());

            if (c.getOffenseTypeId() == null)
                statement.setNull(3, Types.INTEGER);
            else
                statement.setInt(3, c.getOffenseTypeId());

            if (c.getClosedAt() == null) {
                statement.setNull(4, Types.TIMESTAMP);
            } else {
                statement.setTimestamp(
                        4,
                        Timestamp.valueOf(c.getClosedAt())
                );
            }
            statement.setString(5, c.getStatus());
            statement.setString(6, c.getDescription());
            statement.setInt(7, c.getCaseId());

            statement.executeUpdate();

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public void delete(int id) {

        String sql = """
            DELETE FROM cases
            WHERE case_id = ?
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

    public List<String> getOfficers(int caseId) {
        return getNames("""
            SELECT CONCAT(
                p.last_name,' ',
                p.first_name,' ',
                COALESCE(p.middle_name,'')
            )
            FROM police_officers p
            JOIN case_officers co
                ON co.officer_id = p.officer_id
            WHERE co.case_id = ?
            """, caseId);
    }

    public List<String> getDetainees(int caseId) {
        return getNames("""
            SELECT CONCAT(
                d.last_name,' ',
                d.first_name,' ',
                COALESCE(d.middle_name,'')
            )
            FROM detainees d
            JOIN case_detainees cd
                ON cd.detainee_id = d.detainee_id
            WHERE cd.case_id = ?
            """, caseId);
    }

    public List<String> getWitnesses(int caseId) {
        return getNames("""
            SELECT CONCAT(
                w.last_name,' ',
                w.first_name,' ',
                COALESCE(w.middle_name,'')
            )
            FROM witnesses w
            JOIN case_witnesses cw
                ON cw.witness_id = w.witness_id
            WHERE cw.case_id = ?
            """, caseId);
    }

    public List<String> getMeasures(int caseId) {
        return getNames("""
            SELECT
                m.measure_type || ' → ' ||
                d.last_name || ' ' ||
                d.first_name
            FROM measures m
            JOIN detainees d
                ON d.detainee_id = m.detainee_id
            WHERE m.case_id = ?
            """, caseId);
    }

    private List<String> getNames(
            String sql,
            int caseId
    ) {

        List<String> result = new ArrayList<>();

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql)
        ) {

            statement.setInt(1, caseId);

            try (ResultSet rs = statement.executeQuery()) {

                while (rs.next()) {
                    result.add(rs.getString(1));
                }
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return result;
    }
}