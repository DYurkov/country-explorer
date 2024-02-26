import React, { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import { CountryDetail } from "../models/Country";
import { fetchCountryByName } from "../services/CountryService";

const CountryDetails: React.FC = () => {
  const [country, setCountry] = useState<CountryDetail | null>(null);
  const { name } = useParams();

  useEffect(() => {
    const getCountry = async () => {
      if (name) {
        const data = await fetchCountryByName(name);
        setCountry(data);
      }
    };

    getCountry();
  }, [name]);

  if (!country) return <div className="container mt-5">Loading...</div>;

  const { common: commonName, official: officialName } = country.name;
  const currAbbr = country.currencies ? Object.keys(country.currencies)[0] : "";
  const { name: currName, symbol: currSymbol } = currAbbr
    ? country.currencies[currAbbr]
    : { name: "-", symbol: "-" };

  return (
    <div className="container mt-5">
      <div className="card">
        <div className="card-body">
          <h1 className="card-title">
            {commonName} ({officialName})
          </h1>
          <p className="card-text">
            Capital: {country.capital?.join(", ") || "-"}
          </p>
          <p className="card-text">
            Currency: {currName} ({currSymbol})
          </p>
          <p className="card-text">
            Languages:{" "}
            {country.languages
              ? Object.values(country.languages).join(", ")
              : "-"}
          </p>
          <p className="card-text">
            Region: {country.region}{" "}
            {country.subregion ? `(${country.subregion})` : ""}
          </p>
          <div className="flag-container">
            <img
              src={country.flags.png}
              alt={country.flags.alt}
              className="img-fluid mt-3 mb-3"
            />
          </div>
          <div className="map-container">
            <iframe
              title="Google Map"
              width="400"
              height="300"
              frameBorder="0"
              style={{ border: 0 }}
              src={`https://www.google.com/maps?q=${commonName}&output=embed`}
              allowFullScreen
            ></iframe>
          </div>
          <Link to="/" className="btn btn-primary">
            Back to Countries
          </Link>
        </div>
      </div>
    </div>
  );
};

export default CountryDetails;
