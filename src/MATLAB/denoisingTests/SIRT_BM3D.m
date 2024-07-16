
function SIRT_BM3D(Projections,reconImageSIRT)

    nProjections = length(Projections);

    angleStep=360.0/nProjections;

    sizeProjections = length(Projections{1});
  
    R_Thresh = (sizeProjections/2.1)^2;
    centerI= sizeProjections/2;
    centerJ= sizeProjections/2;
    mask=zeros([sizeProjections sizeProjections]);

    for I=1:sizeProjections
            for J=1:sizeProjections
                R= ((I-centerI)^2 + (J-centerJ)^2)/R_Thresh;
                if R>1
                    mask(I,J)=1;
                end
            end
    end
    maskIndex = find(mask==1);

   addition = zeros([sizeProjections sizeProjections]);

    for K=1:50
        diffImage = zeros([sizeProjections sizeProjections]);
        reconImageSIRT(maskIndex)=0;

        for I=1:nProjections
            projection = Projections{I};

            tempImage = imrotate(reconImageSIRT,-1*angleStep*I,'bilinear','crop'); 

            forward = sum(tempImage);

            diffProjections{I}=(projection -forward)/sizeProjections;
        end

        
        for I=1:nProjections
            diff = diffProjections{I}/nProjections;

            for J=1:sizeProjections
               addition(:,J)= diff(J);
            end

            addition=imrotate(addition,angleStep*I,'bilinear','crop');

            diffImage=diffImage + addition;
        end
        
        
        
        imagesc(diffImage);
        drawnow;

        an=1.0/K;

        %regularization = Poisson_denoising_Anscombe_exact_unbiased_inverse(reconImageSIRT) ;
        regularization = doBM3D(reconImageSIRT) ;
        reconImageSIRT=(reconImageSIRT *an + (1-an)* regularization) + diffImage;
    end


    R_Thresh = (sizeProjections/2.2)^2;
    sumV=0;
    cc=0;
    for I=1:sizeProjections
            for J=1:sizeProjections
                R= ((I-centerI)^2 + (J-centerJ)^2)/R_Thresh;
                if R>1 && mask(I,J)~=1
                    sumV=sumV + reconImageSIRT(I,J);
                    cc=cc+1;
                end
            end
    end

    average = sumV/cc;

    reconImageSIRT(maskIndex)=average;


    figure;imagesc(reconImageSIRT);colormap gray;

end


% function [D]= doBM3D(transformed)
%     transformed_sigma=1;   %% this is the standard deviation assumed for the transformed data
% 
%     maxtransformed=max(transformed(:));   %% first put data into [0,1] ...
%     mintransformed=2*sqrt(0+3/8); % min(transformed(:));
%     transformed=(transformed-mintransformed)/(maxtransformed-mintransformed);
%     transformed_sigma=transformed_sigma/(maxtransformed-mintransformed);
% 
%     scale_range=0.7;  %% ... then set data range in [0.15,0.85], to avoid clipping of extreme values
%     scale_shift=(1-scale_range)/2;
%     transformed=transformed*scale_range+scale_shift;
%     transformed_sigma=transformed_sigma*scale_range;
% 
% 
%     % disp(['Min: ',num2str(min(transformed(:))),'   Max: ',num2str(max(transformed(:))),'   sigma*255: ',num2str(transformed_sigma*255)]);
%     if exist('BM3D.m','file')
%         [dummy D]=BM3D(1,transformed,transformed_sigma*255,'np');  % denoise assuming additive white Gaussian noise
%     else
%         disp(' '),disp(' '),disp(' '),disp(' !!!  BM3D denoising software not found  !!!'),disp(' '),disp('     BM3D can be downloaded from http://www.cs.tut.fi/~foi/GCF-BM3D/ '),disp(' '),disp(' ')
%         return
%     end
% 
%     D=(D-scale_shift)/scale_range;
%     D=D*(maxtransformed-mintransformed)+mintransformed;
% end