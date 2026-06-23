package ru.department.zaderhania_web.repository;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;
import ru.department.zaderhania_web.dto.ReportCaseSummaryDto;

import javax.sql.DataSource;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Repository
public class ReportCaseSummaryRepository {

    private final DataSource dataSource;

    @Autowired
    public ReportCaseSummaryRepository(DataSource dataSource) {
        this.dataSource = dataSource;
    }

    public List<ReportCaseSummaryDto> findAll() {

        List<ReportCaseSummaryDto> result = new ArrayList<>();

        String sql = """
                SELECT *
                FROM report_case_summary
                ORDER BY opened_at
                """;

        try (
                Connection connection = dataSource.getConnection();
                PreparedStatement statement = connection.prepareStatement(sql);
                ResultSet rs = statement.executeQuery()
        ) {

            while (rs.next()) {

                ReportCaseSummaryDto dto = new ReportCaseSummaryDto();

                dto.setCaseId(rs.getInt("case_id"));
                dto.setCaseNumber(rs.getString("case_number"));

                dto.setOffenseType(rs.getString("offense_type"));
                dto.setCaseStatus(rs.getString("case_status"));

                dto.setReportNumber(rs.getString("report_number"));

                Timestamp opened = rs.getTimestamp("opened_at");
                Timestamp closed = rs.getTimestamp("closed_at");

                if (opened != null)
                    dto.setOpenedAt(opened.toLocalDateTime());

                if (closed != null)
                    dto.setClosedAt(closed.toLocalDateTime());

                dto.setOfficersCount(rs.getInt("officers_count"));
                dto.setDetaineesCount(rs.getInt("detainees_count"));
                dto.setWitnessesCount(rs.getInt("witnesses_count"));

                dto.setMeasuresCount(rs.getInt("measures_count"));
                dto.setMeasureTypes(rs.getString("measure_types"));

                dto.setDescription(rs.getString("description"));

                result.add(dto);
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return result;
    }
}