

function [D]= doBM3D(transformed)
    transformed_sigma=1;   %% this is the standard deviation assumed for the transformed data

    maxtransformed=max(transformed(:));   %% first put data into [0,1] ...
    mintransformed=2*sqrt(0+3/8); % min(transformed(:));
    transformed=(transformed-mintransformed)/(maxtransformed-mintransformed);
    transformed_sigma=transformed_sigma/(maxtransformed-mintransformed);

    scale_range=0.7;  %% ... then set data range in [0.15,0.85], to avoid clipping of extreme values
    scale_shift=(1-scale_range)/2;
    transformed=transformed*scale_range+scale_shift;
    transformed_sigma=transformed_sigma*scale_range;


    % disp(['Min: ',num2str(min(transformed(:))),'   Max: ',num2str(max(transformed(:))),'   sigma*255: ',num2str(transformed_sigma*255)]);
    if exist('BM3D.m','file')
        [dummy D]=BM3D(1,transformed,transformed_sigma*255,'np');  % denoise assuming additive white Gaussian noise
    else
        disp(' '),disp(' '),disp(' '),disp(' !!!  BM3D denoising software not found  !!!'),disp(' '),disp('     BM3D can be downloaded from http://www.cs.tut.fi/~foi/GCF-BM3D/ '),disp(' '),disp(' ')
        return
    end

    D=(D-scale_shift)/scale_range;
    D=D*(maxtransformed-mintransformed)+mintransformed;
end