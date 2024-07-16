function [yhat]= doBM3DImageFilter(z)
    % Poisson scaling factor
    alpha = .01;
    
    % Gaussian component N(g,sigma^2)
    sigma = 1;
    g = 0.0;
    
        fz = GenAnscombe_forward(z,sigma,alpha,g); % Generalized Anscombe VST (J.L. Starck, F. Murtagh, and A. Bijaoui, Image  Processing  and  Data Analysis, Cambridge University Press, Cambridge, 1998)
        
       
        sigma_den = 1;  % Standard-deviation value assumed after variance-stabiliation
        
        % Scale the image (BM3D processes inputs in [0,1] range)
        scale_range = 1;
        scale_shift = (1-scale_range)/2;
        
        
        maxzans = max(fz(:));
        minzans = min(fz(:));
        fz = (fz-minzans)/(maxzans-minzans);   sigma_den = sigma_den/(maxzans-minzans);
        fz = fz*scale_range+scale_shift;       sigma_den = sigma_den*scale_range;
        
        [dummy D] = BM3D(1,fz); % denoise assuming AWGN
        
        % Scale back to the initial VST range
        D = (D-scale_shift)/scale_range;
        D = D*(maxzans-minzans)+minzans;
        
        % Apply the inverse transformation
        yhat = GenAnscombe_inverse_exact_unbiased(D,sigma,alpha,g);   % exact unbiased inverse
end