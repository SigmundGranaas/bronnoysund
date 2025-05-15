import React from 'react';
import type { StatisticsReportDto } from '../types';

interface StatisticsDisplayProps {
    statistics: StatisticsReportDto | null;
}

const StatCard: React.FC<{ title: string; children: React.ReactNode; className?: string }> = ({ title, children, className = "" }) => (
    <div className={`bg-dark-surface shadow-surface-md rounded-xl p-5 ${className}`}>
        <h3 className="text-base font-semibold text-brand-primary mb-2.5">{title}</h3>
        <div className="border-t border-dark-border my-3"></div>
        <div className="text-sm text-dark-text-primary space-y-1.5">
            {children}
        </div>
    </div>
);

const StatisticsDisplay: React.FC<StatisticsDisplayProps> = ({ statistics }) => {
    if (!statistics) {
        return <p className="mt-8 text-center text-dark-text-secondary">No statistics to display.</p>;
    }

    const { statusCounts, organizationFormDistributionPercentage, employeeDistribution } = statistics;

    return (
        <div className="my-10">
            <h2 className="text-xl sm:text-2xl font-semibold text-dark-text-primary text-center mb-6">
                Statistics
            </h2>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5 sm:gap-6">
                <StatCard title="Status Counts">
                    {Object.entries(statusCounts).length > 0 ?
                        Object.entries(statusCounts).map(([status, count]) => (
                            <div key={status} className="flex justify-between items-center py-0.5">
                                <span className="text-dark-text-secondary">{status}:</span>
                                <span className="font-medium">{count}</span>
                            </div>
                        )) : <p className="text-dark-text-tertiary text-xs">No status data.</p>}
                </StatCard>
                <StatCard title="Organization Form Distribution">
                    {Object.entries(organizationFormDistributionPercentage).length > 0 ?
                        Object.entries(organizationFormDistributionPercentage).map(([form, percentage]) => (
                            <div key={form} className="flex justify-between items-center py-0.5">
                                <span className="text-dark-text-secondary">{form}:</span>
                                <span className="font-medium">{percentage.toFixed(1)}%</span>
                            </div>
                        )) : <p className="text-dark-text-tertiary text-xs">No org. form data.</p>}
                </StatCard>
                <StatCard title="Employee Distribution">
                    <div className="flex justify-between items-center py-0.5"><span className="text-dark-text-secondary">0 Employees:</span> <span className="font-medium">{employeeDistribution.zero}</span></div>
                    <div className="flex justify-between items-center py-0.5"><span className="text-dark-text-secondary">1-9 Employees:</span> <span className="font-medium">{employeeDistribution.oneToNine}</span></div>
                    <div className="flex justify-between items-center py-0.5"><span className="text-dark-text-secondary">10-49 Employees:</span> <span className="font-medium">{employeeDistribution.tenToFortyNine}</span></div>
                    <div className="flex justify-between items-center py-0.5"><span className="text-dark-text-secondary">50+ Employees:</span> <span className="font-medium">{employeeDistribution.fiftyPlus}</span></div>
                </StatCard>
            </div>
        </div>
    );
};

export default StatisticsDisplay;