import React, { useState } from 'react';
import Layout from './components/Layout';
import SplitFileUpload from './components/FileUpload';
import { uploadCompanyCsv, exportCompanyCsv } from './services/api';
import type { ProcessedCompanyDto, StatisticsReportDto } from './types';
import StatisticsDisplay from './components/StatisticDisplay';
import { LoadingState } from './components/states/LoadingState';
import { ErrorState } from './components/states/ErrorState';
import { CompanyTable } from "./components/CompanyTable.tsx";

function App() {
    const [companies, setCompanies] = useState<ProcessedCompanyDto[]>([]);
    const [statistics, setStatistics] = useState<StatisticsReportDto | null>(null);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [csvDownloadUrl, setCsvDownloadUrl] = useState<string | undefined>(undefined);

    const handleFileUpload = async (file: File) => {
        setIsLoading(true);
        setError(null);
        setCompanies([]);
        setStatistics(null);

        const result = await uploadCompanyCsv(file);
        setIsLoading(false);

        if (result.data) {
            setCompanies(result.data.companies);
            setStatistics(result.data.statistics);
        } else if (result.error) {
            setError(result.error);
        }
    };

    const handleFileExport = async (file: File) => {
        setIsLoading(true);
        setError(null);
        setCsvDownloadUrl(undefined);

        const result = await exportCompanyCsv(file);
        setIsLoading(false);

        if (result.data) {
            setCsvDownloadUrl(result.data);
        } else if (result.error) {
            setError(result.error);
        }
    };

    return (
        <Layout>
            <div className="container mx-auto px-4 sm:px-6 lg:px-8 py-6">
                <header className="mb-8">
                    <h1 className="text-2xl sm:text-3xl font-bold text-dark-text-primary text-center">
                        Brønnøysundregisteret CSV parser
                    </h1>
                </header>

                <section className="mb-10">
                    <SplitFileUpload
                        onFileUpload={handleFileUpload}
                        onFileExport={handleFileExport}
                        loading={isLoading}
                        csvDownloadUrl={csvDownloadUrl}
                    />
                </section>

                <MainContent
                    companies={companies}
                    statistics={statistics}
                    isLoading={isLoading}
                    error={error}
                />
            </div>
        </Layout>
    );
}

interface MainContentProps {
    companies: ProcessedCompanyDto[];
    statistics: StatisticsReportDto | null;
    isLoading: boolean;
    error: string | null;
}

const MainContent: React.FC<MainContentProps> =
    ({
         companies,
         statistics,
         isLoading,
         error,
     }) => {
        if (error && !isLoading && companies.length === 0) {
            return <ErrorState message={error} />;
        }

        if (isLoading && companies.length === 0) {
            return <LoadingState />;
        }

        if ((!isLoading || companies.length > 0) &&
            !(companies.length > 0 || statistics)) {
            return null;
        }

        return (
            <>
                <section>
                    <StatisticsDisplay statistics={statistics} />
                </section>
                <section>
                    <CompanyTable companies={companies} />
                </section>
            </>
        );
    };

export default App;