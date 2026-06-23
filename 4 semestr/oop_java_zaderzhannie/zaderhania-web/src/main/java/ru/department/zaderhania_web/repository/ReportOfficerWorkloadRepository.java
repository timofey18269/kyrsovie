package ru.department.zaderhania_web.repository;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Repository;
import ru.department.zaderhania_web.dto.ReportOfficerWorkloadDto;

import javax.sql.DataSource;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

@Repository
public class ReportOfficerWorkloadRepository {

    private final DataSource dataSource;

    @Autowired
    public ReportOfficerWorkloadRepository(
            DataSource dataSource
    ) {
        this.dataSource = dataSource;
    }

    public List<ReportOfficerWorkloadDto> findAll() {

        List<ReportOfficerWorkloadDto> result =
                new ArrayList<>();

        String sql = """
                SELECT *
                FROM report_officer_workload
                ORDER BY cases_count DESC
                """;

        try (
                Connection connection =
                        dataSource.getConnection();

                PreparedStatement statement =
                        connection.prepareStatement(sql);

                ResultSet rs =
                        statement.executeQuery()
        ) {

            while (rs.next()) {

                ReportOfficerWorkloadDto dto =
                        new ReportOfficerWorkloadDto();

                dto.setOfficerId(
                        rs.getInt("officer_id")
                );

                dto.setBadgeNumber(
                        rs.getString("badge_number")
                );

                dto.setFullName(
                        rs.getString("full_name")
                );

                dto.setPositionName(
                        rs.getString("position_name")
                );

                dto.setRankName(
                        rs.getString("rank_name")
                );

                dto.setActive(
                        rs.getBoolean("active")
                );

                dto.setCasesCount(
                        rs.getInt("cases_count")
                );

                dto.setRoles(
                        rs.getString("roles")
                );

                dto.setDetaineesHandled(
                        rs.getInt("detainees_handled")
                );

                dto.setMeasuresInCases(
                        rs.getInt("measures_in_cases")
                );

                result.add(dto);
            }

        } catch (SQLException e) {
            throw new RuntimeException(e);
        }

        return result;
    }
}