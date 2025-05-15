export interface ProcessedCompanyDto {
    orgNo?: string;
    firmaNavn?: string;
    status?: string;
    antallAnsatte?: number;
    organisasjonsformKode?: string;
    naeringskode?: string;
}

export interface EmployeeCountDistributionDto {
    zero: number;
    oneToNine: number;
    tenToFortyNine: number;
    fiftyPlus: number;
}

export interface StatisticsReportDto {
    statusCounts: Record<string, number>;
    organizationFormDistributionPercentage: Record<string, number>;
    employeeDistribution: EmployeeCountDistributionDto;
}

export interface ProcessedCompaniesResultDto {
    companies: ProcessedCompanyDto[];
    statistics: StatisticsReportDto;
}

export interface ApiResponse<T> {
    data?: T;
    error?: string;
}